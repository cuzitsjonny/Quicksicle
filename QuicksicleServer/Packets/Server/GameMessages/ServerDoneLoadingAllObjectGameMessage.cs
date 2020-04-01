using System;
using Quicksicle.IO;
using Quicksicle.Enums;

namespace Quicksicle.Packets
{
    public class ServerDoneLoadingAllObjectGameMessage : BaseGameMessage, ISerializable
    {
        public ServerDoneLoadingAllObjectGameMessage(long objectId) : base(objectId) { }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write(ObjectId);
            packetStream.Write((ushort)GameMessageId.ServerDoneLoadingAllObjects);
        }
    }
}
