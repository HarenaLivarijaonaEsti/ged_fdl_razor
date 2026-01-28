using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Data;
using ged_fdl_razor.Models;

namespace ged_fdl_razor.Pages.Admin.Documents
{
    public class EditModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public EditModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Document Document { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document =  await _context.Documents.FirstOrDefaultAsync(m => m.DocumentID == id);
            if (document == null)
            {
                return NotFound();
            }
            Document = document;
           ViewData["DossierFinancementId"] = new SelectList(_context.DossiersFinancement, "Id", "Titre");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(Document.DocumentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.DocumentID == id);
        }
    }
}
