using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class Section
    {
        public int Id { get; set; }
        public int Distance { get; set; }
        public int Points { get; set; }
        public int Approach { get; set; }
        public bool IsCounted { get; set; }
        public string Description { get; set; }
        public string MountainTrail { get; set; }
        public int LocationFrom { get; set; }
        public int LocationTo { get; set; }
        public int RouteId { get; set; }
        public int OrderNumber { get; set; }

        public virtual Location LocationFromNavigation { get; set; }
        public virtual Location LocationToNavigation { get; set; }
        public virtual Route Route { get; set; }
    }
}
