using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class WorldLoginRequestPacket : IUnserializable
    {
        public long CharacterId;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            CharacterId = packetStream.ReadInt64();
        }
    }
}
