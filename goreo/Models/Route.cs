using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class Route
    {
        public Route()
        {
            BookletsRoutes = new HashSet<BookletsRoute>();
            RoutesSections = new HashSet<RoutesSection>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<BookletsRoute> BookletsRoutes { get; set; }
        public virtual ICollection<RoutesSection> RoutesSections { get; set; }
    }
}
