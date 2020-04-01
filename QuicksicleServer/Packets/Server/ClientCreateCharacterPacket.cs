using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class ClientCreateCharacterPacket : ISerializable
    {
        public WriteOnlyBinaryLdf LdfMap; // Has to be set.

        public void FromBitstream(ReadOnlyBitStream packetStream)
        {
            throw new NotImplementedException();
        }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            byte[] ldfData = LdfMap.ToByteArray();

            packetStream.Write(1 + 4 + ldfData.Length);

            packetStream.Write((byte)0);
            packetStream.Write(LdfMap.Count);
            packetStream.Write(ldfData);
        }
    }
}
