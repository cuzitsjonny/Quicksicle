using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class WorldCharacterListRequestPacket : IUnserializable
    {
        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            // Nothing to unserialize.
        }
    }
}
