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
    public class IndexModel : PageModel
    {
        private readonly ged_fdl_razor.Data.DataContext _context;

        public IndexModel(ged_fdl_razor.Data.DataContext context)
        {
            _context = context;
        }

        public IList<DossierFinancement> DossierFinancement { get;set; } = default!;

        public async Task OnGetAsync()
        {
            DossierFinancement = await _context.DossiersFinancement
                .Include(d => d.Commune).ToListAsync();
        }
    }
}
