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

            ViewData["Sections"] = new SelectList(
                sectionsForFrontend.OrderBy(section => section.Section.LocationFromNavigation.Name), "Section.Id",
                "Display");

            var selectedSections = HttpContext.Session.Keys
                .Where(key => key.Length > 7 && key[..7] == "section")
                .Select(key =>
                    (key[7..],
                        HttpContext.Session.GetInt32(key))
                )
                .OrderBy(tuple => tuple.Item1)
                .Select(tuple =>
                    sectionsForFrontend.FirstOrDefault(section => section.Section.Id == tuple.Item2))
                .ToList();

            ViewData["SelectedSections"] = selectedSections.Select(section => section?.Display);

            ViewData["mountainGroupNo"] = selectedSections.FirstOrDefault()?.Section.LocationFromNavigation
                .LocationsMountainGroups.First().MountainGroupNameNavigation.Number;

            ViewData["points"] = CountPointsFromSections(selectedSections.Select(section => section?.Section));

            return Page();
        }

        [BindProperty] public Section Section { get; set; }

        public IActionResult OnPostSection()
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var sectionNo = HttpContext.Session.GetInt32("no") ?? 0;

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

            var selectedSections = HttpContext.Session.Keys
                .Where(key => key.Length > 7 && key[..7] == "section")
                .Select(key =>
                    (key[7..],
                        _context.Sections.Include(section => section.LocationFromNavigation)
                            .Include(section => section.LocationToNavigation).FirstOrDefault(section =>
                                section.Id == HttpContext.Session.GetInt32(key)))
                )
                .OrderBy(tuple => tuple.Item1)
                .Select(tuple =>
                    tuple.Item2)
                .ToList();

            if (selectedSections.Count == 0)
            {
                // todo (ViewData?)
                return NotFound();
            }

            if (!AreSectionsValid(selectedSections))
            {
                // todo (ViewData?)
                return NotFound();
            }

            var routeName = $"{selectedSections.First().LocationFromNavigation.Name}"
                            + $" - {selectedSections.Last().LocationToNavigation.Name}";

            _context.Routes.Add(new Route { UserId = user.Id, Name = routeName });

            await _context.SaveChangesAsync();

            var routeFromDb = await _context.Routes.FirstOrDefaultAsync(dbRoute => dbRoute.Name == routeName);

            // add the route to the user's booklet
            var isConfirmed = NewRouteData.WasLeaderPresent;

            var booklet = await _context.Booklets.FirstOrDefaultAsync(booklet => booklet.Id == user.BookletId);
            var bookletsRoute = new BookletsRoute
                { EntryDate = NewRouteData.RouteDate, Booklet = booklet, RouteId = routeFromDb.Id, isConfirmed = isConfirmed };
            _context.BookletsRoutes.Add(bookletsRoute);

            // add selectedSections to routes_sections
            var i = 0;
            foreach (var section in selectedSections)
            {
                _context.RoutesSections.Add(new RoutesSection
                    {
                        RouteId = routeFromDb.Id,
                        SectionId = section.Id,
                        OrderNumber = i,
                        IsCounted = NewRouteData
                            .IsCounted // if route is not counted, then all sections should not be counted
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

        private static int CountPointsFromSections(IEnumerable<Section> sections) =>
            sections.GroupBy(section => section.Id)
                .Select(grouping => grouping.First())
                .Aggregate(0, (sum, section) => sum + section.Points);
    }

    public class SectionForFrontend
    {
        public Section Section { get; set; }
        public string Display { get; set; }
    }

    public class NewRouteData
    {
        public DateTime RouteDate { get; set; }
        public bool WasLeaderPresent { get; set; }
        public bool IsCounted { get; set; }
    }
}