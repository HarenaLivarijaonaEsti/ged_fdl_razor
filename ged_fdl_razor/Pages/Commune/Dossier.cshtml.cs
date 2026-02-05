using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ged_fdl_razor.Pages.Commune
{
    // On précise le scheme CommuneCookie
    [Authorize(AuthenticationSchemes = "CommuneCookie")]
    public class DossierModel : PageModel
    {
        private readonly DataContext _context;

        public DossierModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }  // ID de la commune

        public ged_fdl_razor.Models.Commune? Commune { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Commune = await _context.Communes
                .Include(c => c.Dossiers)
                    .ThenInclude(d => d.Documents)
                .Include(c => c.Dossiers)
                    .ThenInclude(d => d.Remarques)
                .FirstOrDefaultAsync(c => c.CommuneID == Id);

            if (Commune == null)
                return NotFound();

            if (Commune.MustChangePassword)
            {
                return RedirectToPage("/Commune/ChangePassword", new { id = Commune.CommuneID });
            }

            return Page();
        }

        // Handler pour Télécharger un document
        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentID == id);
            if (doc == null) return NotFound();

            return File(doc.Contenu, doc.ContentType, doc.Nom + GetExtension(doc.ContentType));
        }

        // Handler pour Ouvrir un document dans le navigateur
        public async Task<IActionResult> OnGetOpenAsync(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentID == id);
            if (doc == null) return NotFound();

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{doc.Nom}{GetExtension(doc.ContentType)}\"");
            return File(doc.Contenu, doc.ContentType);
        }

        private string GetExtension(string contentType)
        {
            return contentType switch
            {
                "application/pdf" => ".pdf",
                "application/msword" => ".doc",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
                "application/vnd.ms-excel" => ".xls",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
                _ => ""
            };
        }

        // Déconnexion correcte du cookie Commune
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("CommuneCookie");
            return RedirectToPage("/Commune/Login");
        }
    }
}
