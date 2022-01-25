using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using goreo;
using Microsoft.AspNetCore.Authorization;

namespace goreo.Pages.Routes
{
    [Authorize(Policy = "MustBeLeader")]
    public class PendingModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public PendingModel(goreo.postgresContext context)
        {
            _context = context;
        }

        public IList<BookletsRoute> BookletsRoutes { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userClaims = ((ClaimsIdentity)User.Identity)?.Claims;
            if (userClaims == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            var username = userClaims.Where(claim =>
                claim.Type == ClaimTypes.Name).Select(claim => claim.Value).SingleOrDefault();

            BookletsRoutes = await _context.BookletsRoutes
                .Where(bookletsRoute => !bookletsRoute.isConfirmed)
                .Include(b => b.Booklet)
                .ThenInclude(booklet => booklet.User)
                .Where(bookletsRoute => bookletsRoute.Booklet.User.Username != username)
                .Include(b => b.Route).ToListAsync();

            return Page();
        }
    }
}
