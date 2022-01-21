using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class BookletsBadge
    {
        public int BookletId { get; set; }
        public int BadgeId { get; set; }
        public DateTime EarnDate { get; set; }

        public virtual Badge Badge { get; set; }
        public virtual Booklet Booklet { get; set; }
    }
}
