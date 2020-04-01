using System;
using System.Collections.Generic;

namespace Quicksicle.Resources
{
    public class ZoneChecksumCache
    {
        Dictionary<ushort, uint> checksums;

        public ZoneChecksumCache()
        {
            this.checksums = new Dictionary<ushort, uint>();

            LoadChecksums();
        }

        private void LoadChecksums()
        {
            checksums.Add(1000, 548931708);
        }

        public uint GetZoneChecksum(ushort zoneId)
        {
            if (checksums.ContainsKey(zoneId))
            {
                return checksums[zoneId];
            }

            return 0;
        }
    }
}
