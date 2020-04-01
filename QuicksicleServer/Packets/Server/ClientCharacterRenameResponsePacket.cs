using System;
using Quicksicle.IO;
using Quicksicle.Enums;

namespace Quicksicle.Packets
{
    public class ClientCharacterRenameResponsePacket : ISerializable
    {
        public CharacterRenameResult CharacterRenameResult; // Has to be set.

        public void FromBitstream(ReadOnlyBitStream packetStream)
        {
            throw new NotImplementedException();
        }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write((byte)CharacterRenameResult);
        }
    }
}
