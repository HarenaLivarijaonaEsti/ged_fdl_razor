using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ged_fdl_razor.Pages.Admin.Nav
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToPage("/Admin/Index");
        }
    }
}
