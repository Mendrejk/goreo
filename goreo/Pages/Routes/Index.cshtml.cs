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
    [Authorize(Policy = "MustBeUserOrLeader")]
    public class IndexModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public IndexModel(postgresContext context)
        {
            _context = context;
        }

        public User IndexedUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userClaims = ((ClaimsIdentity)User.Identity)?.Claims;
            if (userClaims == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            var username = userClaims.Where(claim =>
                claim.Type == ClaimTypes.Name).Select(claim => claim.Value).SingleOrDefault();

            var user = await _context.Users
                .Include(user => user.Booklet)
                .Include(user => user.Routes)
                .ThenInclude(route => route.BookletsRoutes)
                .FirstOrDefaultAsync(user => user.Username == username);
            if (user == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            IndexedUser = user;

            return Page();
        }
    }
}