using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace goreo.Pages.Routes
{
    [Authorize(Policy = "MustBeUser")]
    public class IndexModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public IndexModel(postgresContext context)
        {
            _context = context;
        }

        public IList<Route> Routes { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userClaims = ((ClaimsIdentity)User.Identity)?.Claims;
            if (userClaims == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            var username = userClaims.Where(claim =>
                claim.Type == ClaimTypes.Name).Select(claim => claim.Value).SingleOrDefault();

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
            if (user == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            Routes = user.Routes.ToList();

            return Page();
        }
    }
}