using ged_fdl_razor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- DbContext ---
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Authentification Cookie séparée pour Admin et Commune ---
builder.Services.AddAuthentication()
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/Admin/Index";
        options.AccessDeniedPath = "/Admin/Index";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    })
    .AddCookie("CommuneCookie", options =>
    {
        options.LoginPath = "/Commune/Login";
        options.AccessDeniedPath = "/Commune/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

// --- Razor Pages avec conventions ---
builder.Services.AddRazorPages(options =>
{
    // Toutes les pages du dossier Admin nécessitent Auth Admin
    options.Conventions.AuthorizeFolder("/Admin", "AdminPolicy");

    // Page de login Admin accessible sans auth
    options.Conventions.AllowAnonymousToPage("/Admin/Index");

    // Toutes les pages du dossier Commune nécessitent Auth Commune
    options.Conventions.AuthorizeFolder("/Commune", "CommunePolicy");

    // Page de login Commune accessible sans auth
    options.Conventions.AllowAnonymousToPage("/Commune/Login");
});

// --- Ajouter les politiques pour séparer les rôles ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.AddAuthenticationSchemes("AdminCookie");
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(System.Security.Claims.ClaimTypes.Role, "Admin");
    });

    options.AddPolicy("CommunePolicy", policy =>
    {
        policy.AddAuthenticationSchemes("CommuneCookie");
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

// --- Initialisation DB ---
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

// --- Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentification & autorisation
app.UseAuthentication();
app.UseAuthorization();

// Redirection index.html -> /
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/index.html")
    {
        context.Response.Redirect("/");
        return;
    }
    await next();
});

app.MapRazorPages();
app.Run();
