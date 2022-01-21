using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class BookletsRoute
    {
        public int BookletId { get; set; }
        public int RouteId { get; set; }
        public DateTime EntryDate { get; set; }

        public virtual Booklet Booklet { get; set; }
        public virtual Route Route { get; set; }
    }
}
