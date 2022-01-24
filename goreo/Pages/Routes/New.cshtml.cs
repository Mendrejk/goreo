using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            ViewData["User"] = user;

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
                            $"{section.LocationFromNavigation.LocationsMountainGroups.First().MountainGroupNameNavigation.Number}: "
                            + $"{section.LocationFromNavigation.Name} - {section.LocationToNavigation.Name}"
                    }
                );

            ViewData["Sections"] = new SelectList(sectionsForFrontend, "Section.Id", "Display");

            ViewData["SelectedSections"] = HttpContext.Session.Keys
                .Where(key => key.Length > 7 && key[..7] == "section")
                .Select(key =>
                    (key[7..],
                        HttpContext.Session.GetInt32(key))
                )
                .OrderBy(tuple => tuple.Item1)
                .Select(tuple =>
                    sectionsForFrontend.FirstOrDefault(section => section.Section.Id == tuple.Item2)?.Display)
                .ToList();

            return Page();
        }

        [BindProperty] public Section Section { get; set; }
        public IActionResult OnPostSection()
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var sectionNo = HttpContext.Session.GetInt32("no")?? 0;

            HttpContext.Session.SetInt32($"section{sectionNo}", Section.Id);

            HttpContext.Session.SetInt32("no", sectionNo + 1);

            return RedirectToPage("/Routes/New");
        }

        [BindProperty] public NewRouteData NewRouteData { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostRouteAsync()
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

            if (NewRouteData.SelectedSections.Count == 0)
            {
                // todo (ViewData?)
                return Page();
            }
            
            if (!AreSectionsValid(NewRouteData.SelectedSections))
            {
                // todo (ViewData?)
                return Page();
            }

            var routeName = $"{NewRouteData.SelectedSections.First().LocationFromNavigation.Name}"
                            + $" - {NewRouteData.SelectedSections.Last().LocationToNavigation.Name}";

            var route = new Route { UserId = user.Id, Name = routeName};
            _context.Routes.Add(route);

            // add the route to the user's booklet
            var isConfirmed = NewRouteData.WasLeaderPresent;

            var booklet = await _context.Booklets.FirstOrDefaultAsync(booklet => booklet.Id == user.BookletId);
            var bookletsRoute = new BookletsRoute
                { EntryDate = NewRouteData.RouteDate, Booklet = booklet, Route = route, isConfirmed = isConfirmed };
            _context.BookletsRoutes.Add(bookletsRoute);

            // add selectedSections to routes_sections
            var i = 0;
            foreach (var section in NewRouteData.SelectedSections)
            {
                _context.RoutesSections.Add(new RoutesSection
                    {
                        RouteId = route.Id,
                        SectionId = section.Id,
                        OrderNumber = i,
                        IsCounted = NewRouteData.IsCounted // if route is not counted, then all sections should not be counted
                    }
                );

                i++;
            }

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

        private static bool AreSectionsValid(IEnumerable<Section> sections)
        {
            // every section must begin where the previous one has ended
            int? previousEndId = null;

            return sections.All(section =>
                {
                    var isConnected = (previousEndId == null || section.LocationFrom == previousEndId);

                    previousEndId = section.LocationTo;

                    return isConnected;
                }
            );
        }
    }

    public class SectionForFrontend
    {
        public Section Section { get; set; }
        public string Display { get; set; }
    }

    public class NewRouteData
    {
        public List<Section> SelectedSections { get; set; }
        public DateTime RouteDate { get; set; }
        public bool WasLeaderPresent { get; set; }
        public bool IsCounted { get; set; }
    }
}