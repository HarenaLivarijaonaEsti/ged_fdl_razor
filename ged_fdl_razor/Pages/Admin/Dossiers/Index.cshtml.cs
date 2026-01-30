using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ged_fdl_razor.Pages.Admin.Dossiers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;

        public IndexModel(DataContext context)
        {
            _context = context;
        }

        public IList<DossierFinancement> DossierFinancement { get; set; } = new List<DossierFinancement>();

        // 🔑 communeId reçu depuis Communes/Index
        public async Task OnGetAsync(int? communeId)
        {
            IQueryable<DossierFinancement> query = _context.DossiersFinancement
                .Include(d => d.Commune);

            // 🔎 Filtrage par commune
            if (communeId.HasValue)
            {
                query = query.Where(d => d.CommuneId == communeId);
            }

            DossierFinancement = await query.ToListAsync();
        }
    }
}
