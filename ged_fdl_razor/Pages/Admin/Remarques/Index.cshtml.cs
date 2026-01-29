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
    public class IndexModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public IndexModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        public IList<Remarque> Remarque { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Remarque = await _context.Remarques
                .Include(r => r.Commune)
                .Include(r => r.DossierFinancement).ToListAsync();
        }
    }
}
