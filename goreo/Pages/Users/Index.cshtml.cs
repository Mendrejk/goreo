using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using goreo;

namespace goreo.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public IndexModel(goreo.postgresContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; }

        public async Task OnGetAsync()
        {
            User = await _context.Users
                .Include(u => u.Booklet).ToListAsync();
        }
    }
}
