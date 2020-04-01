using System;
using System.IO;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class WorldCharacterDeleteRequestPacket : IUnserializable
    {
        public long CharacterId;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            CharacterId = packetStream.ReadInt64();
        }
    }
}
