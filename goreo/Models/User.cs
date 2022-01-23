using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace goreo
{
    public partial class User
    {
        public User()
        {
            Routes = new HashSet<Route>();
        }

        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string ProfileImage { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string CityOfResidence { get; set; }
        [Required]
        public bool IsLeader { get; set; }
        public string LeaderIdNo { get; set; }
        public bool IsAdmin { get; set; }
        public int? BookletId { get; set; }

        public virtual Booklet Booklet { get; set; }
        public virtual ICollection<Route> Routes { get; set; }

        public string DetermineRole() =>
            (IsAdmin, IsLeader) switch
            {
                (true, false) =>
                    Roles.Administrator,
                (false, true) => Roles.Leader,
                _ => Roles.User
            };

        public string GetRoleForFrontend() =>
            DetermineRole() switch
            {
                Roles.Leader => "Przodownik",
                _ => "Turysta"
            };

        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Leader = "Leader";
            public const string User = "User";
        }
    }
}