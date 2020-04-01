using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class WorldLevelLoadCompletePacket : IUnserializable
    {
        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            // Nothing to unserialize.
        }
    }
}
