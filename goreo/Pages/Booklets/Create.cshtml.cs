using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using goreo;

namespace goreo.Pages.Booklets
{
    public class CreateModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public CreateModel(goreo.postgresContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Booklet Booklet { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Booklets.Add(Booklet);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
