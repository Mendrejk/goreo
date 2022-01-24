using System;
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
    public class NewModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public NewModel(goreo.postgresContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            var userClaims = ((ClaimsIdentity)User.Identity)?.Claims;
            if (userClaims == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            var username = userClaims.Where(claim =>
                claim.Type == ClaimTypes.Name).Select(claim => claim.Value).SingleOrDefault();

            var user = await _context.Users.Include(user => user.Routes)
                .FirstOrDefaultAsync(user => user.Username == username);
            if (user == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            Route = new Route { User = user };

            return Page();
        }

        [BindProperty] public Route Route { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Routes.Add(Route);

            // add the route to the user's booklet
            var booklet = await _context.Booklets.FirstOrDefaultAsync(booklet => booklet.Id == Route.UserId);
            var bookletsRoute = new BookletsRoute { EntryDate = DateTime.Now, Booklet = booklet, Route = Route };
            _context.BookletsRoutes.Add(bookletsRoute);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Routes/Index");
        }
    }
}