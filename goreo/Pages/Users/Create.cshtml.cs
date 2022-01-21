using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using goreo;

namespace goreo.Pages.Users
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
        ViewData["BookletId"] = new SelectList(_context.Booklets, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public User User { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
