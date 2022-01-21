using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class Location
    {
        public Location()
        {
            LocationsMountainGroups = new HashSet<LocationsMountainGroup>();
            SectionLocationFromNavigations = new HashSet<Section>();
            SectionLocationToNavigations = new HashSet<Section>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public decimal? XCoordinate { get; set; }
        public decimal? YCoordinate { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public virtual ICollection<LocationsMountainGroup> LocationsMountainGroups { get; set; }
        public virtual ICollection<Section> SectionLocationFromNavigations { get; set; }
        public virtual ICollection<Section> SectionLocationToNavigations { get; set; }
    }
}
