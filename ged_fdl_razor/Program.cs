using ged_fdl_razor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- Ajouter DbContext ---
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Ajouter l'authentification Cookie ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin"; // Page de login
        options.AccessDeniedPath = "/Admin"; // Optional : page en cas d'accès refusé
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Durée du cookie
    });

// --- Ajouter Razor Pages ---
builder.Services.AddRazorPages();

var app = builder.Build();

// --- Initialiser la DB (équivalent CreateDbIfNotExists) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// --- Middleware pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentification et autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
