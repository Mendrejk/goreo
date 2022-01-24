using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task OnGetAsync()
        {
            BookletsRoutes = await _context.BookletsRoutes
                .Where(bookletsRoute => !bookletsRoute.isConfirmed)
                .Include(b => b.Booklet)
                .ThenInclude(booklet => booklet.User)
                .Include(b => b.Route).ToListAsync();
        }
    }
}
