using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class RoutesSection
    {
        public int RouteId { get; set; }
        public int SectionId { get; set; }
        public int OrderNumber { get; set; }
        public bool IsCounted { get; set; }

        public virtual Route Route { get; set; }
        public virtual Section Section { get; set; }
    }
}