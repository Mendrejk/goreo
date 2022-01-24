using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var user = GetUserFromClaims().Result;
            if (user == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            Route = new Route { User = user };

            var sectionsForFrontend = _context.Sections
                .Include(section => section.LocationFromNavigation)
                .ThenInclude(locationFromNavigation => locationFromNavigation.LocationsMountainGroups)
                .ThenInclude(locationsMountainGroup => locationsMountainGroup.MountainGroupNameNavigation)
                .Include(section => section.LocationToNavigation)
                .Select(section =>
                    new SectionForFrontend
                    {
                        Section = section,
                        Display =
                            $"{section.LocationFromNavigation.LocationsMountainGroups.First().MountainGroupNameNavigation.Number}: {section.LocationFromNavigation.Name} - {section.LocationToNavigation.Name}"
                    }
                );

            ViewData["Sections"] = new SelectList(sectionsForFrontend, "Section.Id", "Display");

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

            var user = GetUserFromClaims().Result;
            if (user == null)
            {
                return RedirectToPage("/Users/Logout");
            }

            Route.User = user;
            Route.UserId = user.Id;

            _context.Routes.Add(Route);

            // add the route to the user's booklet
            var booklet = await _context.Booklets.FirstOrDefaultAsync(booklet => booklet.Id == Route.UserId);
            var bookletsRoute = new BookletsRoute { EntryDate = DateTime.Now, Booklet = booklet, Route = Route };
            _context.BookletsRoutes.Add(bookletsRoute);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Routes/Index");
        }

        private async Task<User> GetUserFromClaims()
        {
            var userClaims = ((ClaimsIdentity)User.Identity)?.Claims;
            if (userClaims == null)
            {
                return null;
            }

            var username = userClaims.Where(claim =>
                claim.Type == ClaimTypes.Name).Select(claim => claim.Value).SingleOrDefault();

            var user = await _context.Users.Include(user => user.Routes)
                .FirstOrDefaultAsync(user => user.Username == username);

            return user;
        }
    }

    public class SectionForFrontend
    {
        public Section Section { get; set; }
        public string Display  { get; set; }
    }
}