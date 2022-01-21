using System;
using System.Collections.Generic;

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
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string CityOfResidence { get; set; }
        public bool IsLeader { get; set; }
        public string LeaderIdNo { get; set; }
        public bool IsAdmin { get; set; }
        public int? BookletId { get; set; }

        public virtual Booklet Booklet { get; set; }
        public virtual ICollection<Route> Routes { get; set; }
    }
}
