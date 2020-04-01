using System;
using Quicksicle.IO;
using Quicksicle.Enums;

namespace Quicksicle.Packets
{
    public class GeneralDisconnectNotifyPacket : ISerializable
    {
        public DisconnectReason DisconnectReason = DisconnectReason.NoReason;

        public void FromBitstream(ReadOnlyBitStream packetStream)
        {
            throw new NotImplementedException();
        }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write((uint)DisconnectReason);
        }
    }
}
