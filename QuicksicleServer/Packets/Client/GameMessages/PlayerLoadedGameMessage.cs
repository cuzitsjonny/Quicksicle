using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class PlayerLoadedGameMessage : BaseGameMessage, IUnserializable
    {
        public long PlayerId;

        public PlayerLoadedGameMessage(long objectId) : base(objectId) { }

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            PlayerId = packetStream.ReadInt64();
        }
    }
}
