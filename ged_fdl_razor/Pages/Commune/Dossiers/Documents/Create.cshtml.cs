using ged_fdl_razor.Data;
using ged_fdl_razor.Enums;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ged_fdl_razor.Pages.Commune.Dossiers.Documents
{
    public class CreateModel : PageModel
    {
        private readonly DataContext _context;

        public CreateModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int DossierId { get; set; }

        [BindProperty]
        public Document Document { get; set; }

        [BindProperty]
        public IFormFile UploadFile { get; set; }

        public int CommuneId { get; set; }

        // Types de documents encore disponibles
        public List<TypeDocument> TypesDisponibles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var dossier = await _context.DossiersFinancement
                .FirstOrDefaultAsync(d => d.Id == DossierId);

            if (dossier == null)
                return NotFound();

            CommuneId = dossier.CommuneId;

            await ChargerTypesDisponibles();

            Document = new Document();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dossier = await _context.DossiersFinancement
                .FirstOrDefaultAsync(d => d.Id == DossierId);

            if (dossier == null)
                return NotFound();

            CommuneId = dossier.CommuneId;

            await ChargerTypesDisponibles();

            if (!ModelState.IsValid)
                return Page();

            if (UploadFile != null)
            {
                using var ms = new MemoryStream();
                await UploadFile.CopyToAsync(ms);

                Document.Contenu = ms.ToArray();
                Document.Taille = ms.Length;
                Document.ContentType = UploadFile.ContentType;
            }

            Document.DossierFinancementId = DossierId;
            Document.DateUpload = DateTime.UtcNow;

            _context.Documents.Add(Document);

            // Mise à jour du flag
            var commune = await _context.Communes
                .FirstOrDefaultAsync(c => c.CommuneID == dossier.CommuneId);

            if (commune != null)
                commune.HasNewDocuments = true;

            // ================================
            // ✅ NOUVEAU : Mise à jour statut
            // ================================

            // Tous les types requis
            var typesRequis = Enum.GetValues(typeof(TypeDocument))
                .Cast<TypeDocument>()
                .ToList();

            // Types déjà présents en base
            var typesDeja = await _context.Documents
                .Where(d => d.DossierFinancementId == DossierId)
                .Select(d => d.Type)
                .ToListAsync();

            // Ajouter le type qu'on vient d'insérer
            typesDeja.Add(Document.Type);

            bool dossierComplet = !typesRequis.Except(typesDeja).Any();

            if (dossier.Statut == StatutDossier.Brouillon && dossierComplet)
            {
                dossier.Statut = StatutDossier.Soumis;
            }

            // ================================

            await _context.SaveChangesAsync();

            return RedirectToPage("/Commune/Dossier", new { id = CommuneId });
        }

        // Méthode centrale de filtrage
        private async Task ChargerTypesDisponibles()
        {
            var typesDejaUtilises = await _context.Documents
                .Where(d => d.DossierFinancementId == DossierId)
                .Select(d => d.Type)
                .ToListAsync();

            TypesDisponibles = Enum.GetValues(typeof(TypeDocument))
                .Cast<TypeDocument>()
                .Except(typesDejaUtilises)
                .ToList();
        }
    }
}