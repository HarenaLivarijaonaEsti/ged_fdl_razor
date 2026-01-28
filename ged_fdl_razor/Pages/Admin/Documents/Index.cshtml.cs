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
    public class IndexModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public IndexModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        public IList<Document> Document { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Document = await _context.Documents
                .Include(d => d.DossierFinancement).ToListAsync();
        }
    }
}
