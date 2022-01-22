using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace goreo.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly goreo.postgresContext _context;

        public LoginModel(goreo.postgresContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        // public async Task<IActionResult> OnPostAsync()
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return Page();
        //     }
        //
        //     var user =
        //         (await _context.Users.ToListAsync()).Find(user => user.Username == Credentials.Username);
        //     if (user == null)
        //     {
        //         // TODO : add "user does not exist" page
        //         return Page();
        //     }
        //
        //     // TODO : add hashing :)
        //     var passwordMatches = Credentials.Password == user.Password;
        //     if (!passwordMatches)
        //     {
        //         return Page();
        //     }
        //     
        //     
        // }


        [BindProperty] public LoginCredentials Credentials { get; set; }
    }

    public class LoginCredentials
    {
        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}