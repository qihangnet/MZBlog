using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MZBlog.Web.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}