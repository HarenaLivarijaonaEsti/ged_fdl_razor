using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Data;
using ged_fdl_razor.Models;

namespace ged_fdl_razor.Pages.Admin.Documents
{
    public class DetailsModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public DetailsModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        public Document Document { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FirstOrDefaultAsync(m => m.DocumentID == id);

            if (document is not null)
            {
                Document = document;

                return Page();
            }

            return NotFound();
        }
    }
}
