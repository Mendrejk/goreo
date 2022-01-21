using System;
using System.Collections.Generic;

#nullable disable

namespace goreo
{
    public partial class LocationsMountainGroup
    {
        public int LocationId { get; set; }
        public string MountainGroupName { get; set; }

        public virtual Location Location { get; set; }
        public virtual MountainGroup MountainGroupNameNavigation { get; set; }
    }
}
