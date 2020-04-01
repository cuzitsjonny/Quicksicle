using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class WorldCharacterRenameRequestPacket : IUnserializable
    {
        public long CharacterId;
        public string NewName;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            CharacterId = packetStream.ReadInt64();
            NewName = packetStream.ReadWideString(33);
        }
    }
}
