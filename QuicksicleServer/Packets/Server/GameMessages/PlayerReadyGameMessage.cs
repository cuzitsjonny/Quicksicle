using System;
using Quicksicle.IO;
using Quicksicle.Enums;

namespace Quicksicle.Packets
{
    public class PlayerReadyGameMessage : BaseGameMessage, ISerializable
    {
        public PlayerReadyGameMessage(long objectId) : base(objectId) { }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write(ObjectId);
            packetStream.Write((ushort)GameMessageId.PlayerReady);
        }
    }
}
