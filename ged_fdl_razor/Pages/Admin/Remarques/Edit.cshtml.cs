using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ged_fdl_razor.Pages.Admin.Remarques
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public EditModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Remarque Remarque { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var remarque =  await _context.Remarques.FirstOrDefaultAsync(m => m.RemarqueID == id);
            if (remarque == null)
            {
                return NotFound();
            }
            Remarque = remarque;
           ViewData["CommuneId"] = new SelectList(_context.Communes, "CommuneID", "District");
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

            _context.Attach(Remarque).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RemarqueExists(Remarque.RemarqueID))
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

        private bool RemarqueExists(int id)
        {
            return _context.Remarques.Any(e => e.RemarqueID == id);
        }
    }
}
