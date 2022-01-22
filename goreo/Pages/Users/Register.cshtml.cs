using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace goreo.Pages.Users
{
    public class RegisterModel : PageModel
    {
        private readonly goreo.postgresContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public RegisterModel(goreo.postgresContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult OnGet()
        {
            ViewData["BookletId"] = new SelectList(_context.Booklets, "Id", "Id");
            return Page();
        }

        [BindProperty] public Registrant Registrant { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Save image to wwwroot/uploads
            var wwwRootPath = _hostEnvironment.WebRootPath;

            if (Registrant.ImageFile != null) // if null, then the default image shall be used
            {
                var fileName = Path.GetFileNameWithoutExtension(Registrant.ImageFile.FileName);
                var extension = Path.GetExtension(Registrant.ImageFile.FileName);
                
                fileName += DateTime.Now.ToString("yymmssfff") + extension;
                Registrant.User.ProfileImage = fileName;
                
                var path = Path.Combine(wwwRootPath + "/profilePictures", fileName);

                await using var fileStream = new FileStream(path, FileMode.Create);
                await Registrant.ImageFile.CopyToAsync(fileStream);
            }

            // Insert the new user into the db
            _context.Users.Add(Registrant.User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

    public class Registrant
    {
        [Required]
        public User User { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}