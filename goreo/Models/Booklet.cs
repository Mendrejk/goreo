using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class Booklet
    {
        public Booklet()
        {
            BookletsBadges = new HashSet<BookletsBadge>();
            BookletsRoutes = new HashSet<BookletsRoute>();
        }

        public int Id { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<BookletsBadge> BookletsBadges { get; set; }
        public virtual ICollection<BookletsRoute> BookletsRoutes { get; set; }
    }
}
