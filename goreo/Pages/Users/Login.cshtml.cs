using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        public IActionResult OnGet()
        {
            var isLoggedIn = HttpContext.User.Identity is { IsAuthenticated: true };

            // ReSharper disable once InvertIf
            if (isLoggedIn)
            {
                var userClaims = ((ClaimsIdentity)User.Identity)?.Claims;
                if (userClaims == null)
                {
                    return RedirectToPage("/Users/Logout");
                }

                var role = userClaims.Where(claim =>
                    claim.Type == ClaimTypes.Role).Select(claim => claim.Value).SingleOrDefault();

                return role switch
                {
                    null => RedirectToPage("/Users/Logout"),
                    "User" => RedirectToPage("/Routes/Index"),
                    _ => RedirectToPage("/Users/Index")
                };
            }

            // default path - render the login page
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user =
                (await _context.Users.ToListAsync()).Find(user => user.Username == Credentials.Username);
            if (user == null)
            {
                // TODO : add "user does not exist" page
                return Page();
            }

            // TODO : add hashing :)
            var passwordMatches = Credentials.Password == user.Password;
            if (!passwordMatches)
            {
                return Page();
            }

            var userRole = user.DetermineRole();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, userRole)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (userRole == "User")
            {
                return RedirectToPage("/Routes/Index");
            }
            
            return RedirectToPage("/Users/Index");
        }

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