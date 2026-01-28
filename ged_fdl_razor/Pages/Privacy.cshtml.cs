using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ged_fdl_razor.Pages
{
    public class PrivacyModel : PageModel
    {
        // Propriété publique pour exposer la commune à la page Razor
        public Commune TestCommune { get; set; } = default!;
        DataContext dataContext;
        
        public PrivacyModel(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public void OnGet()
        {
            // Création d'une commune test
            //TestCommune = new Commune
            //{
            //    Nom = "Commune de Test",
            //    Email = "test@commune.gov",
            //    PasswordHash = "hash_dummy",
            //    District = "District 1",
            //    Region = "Région Test",
            //    MustChangePassword = true
            //};
            //dataContext.Communes.Add(TestCommune);
            //dataContext.SaveChanges();
        }
    }
}
