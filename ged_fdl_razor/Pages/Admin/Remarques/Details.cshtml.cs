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
    public class DetailsModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public DetailsModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

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
    }
}
