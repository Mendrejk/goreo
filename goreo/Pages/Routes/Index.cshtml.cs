using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace goreo.Pages.Routes
{
    [Authorize(Policy = "MustBeUser")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}