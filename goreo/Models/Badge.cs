using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class Badge
    {
        public Badge()
        {
            BookletsBadges = new HashSet<BookletsBadge>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public virtual ICollection<BookletsBadge> BookletsBadges { get; set; }
    }
}
