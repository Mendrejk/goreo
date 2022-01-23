using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace goreo.Pages.Routes
{
    public class NewModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public NewModel(goreo.postgresContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CityOfResidence");
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
            await _context.SaveChangesAsync();

            return RedirectToPage("/Routes/Index");
        }
    }
}