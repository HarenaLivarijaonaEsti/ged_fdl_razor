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

namespace ged_fdl_razor.Pages.Admin.Dossiers
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public DetailsModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        public DossierFinancement DossierFinancement { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossierfinancement = await _context.DossiersFinancement.FirstOrDefaultAsync(m => m.Id == id);

            if (dossierfinancement is not null)
            {
                DossierFinancement = dossierfinancement;

                return Page();
            }

            return NotFound();
        }
    }
}
