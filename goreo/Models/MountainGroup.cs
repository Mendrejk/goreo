using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class MountainGroup
    {
        public MountainGroup()
        {
            LocationsMountainGroups = new HashSet<LocationsMountainGroup>();
        }

        public string Name { get; set; }

        public virtual ICollection<LocationsMountainGroup> LocationsMountainGroups { get; set; }
    }
}
