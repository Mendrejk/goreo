using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace goreo.Pages.Users
{
    [Authorize(Policy = "MustBeAdmin")]
    public class IndexModel : PageModel
    {
        private readonly goreo.postgresContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public IndexModel(goreo.postgresContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IList<UserWithImagePath> UsersWithImagePath { get; set; }

        public async Task OnGetAsync()
        {
            var users = await _context.Users
                .Include(u => u.Booklet).ToListAsync();

            UsersWithImagePath = users.Select(user =>
                {
                    var imagePath = user.ProfileImage == null
                                    || !System.IO.File.Exists(Path.Combine(
                                        _hostEnvironment.WebRootPath + "/profilePictures",
                                        user.ProfileImage))
                        ? "~/photos/DefaultProfileImage.png"
                        : "~/profilePictures/" + user.ProfileImage;

                    return new UserWithImagePath { User = user, ImagePath = imagePath };
                }
            ).ToList();
        }
    }
}