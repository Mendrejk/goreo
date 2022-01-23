using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace goreo.Pages.Users
{
    [Authorize(Policy = "MustBeAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly goreo.postgresContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DeleteModel(goreo.postgresContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User = await _context.Users
                .Include(u => u.Booklet).FirstOrDefaultAsync(m => m.Id == id);

            if (User == null)
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

            User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                return RedirectToPage("./Index");
            }

            // delete image from wwwroot/uploads
            if (User.ProfileImage != null)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "profilePictures", User.ProfileImage);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // delete the record
            _context.Users.Remove(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
