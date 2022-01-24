using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace goreo.Pages.Routes
{
    [Authorize(Policy = "MustBeLeader")]
    public class Accept : PageModel
    {
        private readonly goreo.postgresContext _context;

        public Accept(postgresContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookletsRoute = await _context.BookletsRoutes
                .FirstOrDefaultAsync(bookletsRoute => bookletsRoute.RouteId == id);
            if (bookletsRoute == null)
            {
                return NotFound();
            }

            bookletsRoute.isConfirmed = true;

            _context.Attach(bookletsRoute).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BookletsRoutes
                    .Any(route => route.RouteId == id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("/Routes/Pending");
        }
    }
}