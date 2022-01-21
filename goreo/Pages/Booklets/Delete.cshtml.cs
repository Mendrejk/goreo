using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using goreo;

namespace goreo.Pages.Booklets
{
    public class DeleteModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public DeleteModel(goreo.postgresContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booklet Booklet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Booklet = await _context.Booklets.FirstOrDefaultAsync(m => m.Id == id);

            if (Booklet == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Booklet = await _context.Booklets.FindAsync(id);

            if (Booklet != null)
            {
                _context.Booklets.Remove(Booklet);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
