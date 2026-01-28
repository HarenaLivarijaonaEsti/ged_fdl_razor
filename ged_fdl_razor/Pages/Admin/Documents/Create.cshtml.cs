using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ged_fdl_razor.Data;
using ged_fdl_razor.Models;

namespace ged_fdl_razor.Pages.Admin.Documents
{
    public class CreateModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public CreateModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DossierFinancementId"] = new SelectList(_context.DossiersFinancement, "Id", "Titre");
            return Page();
        }

        [BindProperty]
        public Document Document { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Documents.Add(Document);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
