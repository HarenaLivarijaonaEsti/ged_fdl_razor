using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ged_fdl_razor.Pages.Admin.Remarques
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public DeleteModel(ged_fdl_razor.Data.DataContext context)
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

            var remarque = await _context.Remarques.FirstOrDefaultAsync(m => m.RemarqueID == id);

            if (remarque is not null)
            {
                Remarque = remarque;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var remarque = await _context.Remarques.FindAsync(id);
            if (remarque != null)
            {
                Remarque = remarque;
                _context.Remarques.Remove(Remarque);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
