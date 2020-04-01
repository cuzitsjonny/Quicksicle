using System;
using Quicksicle.IO;
using Jitter.LinearMath;

namespace Quicksicle.Packets
{
    public class ClientLoadStaticZonePacket : ISerializable
    {
        public ushort ZoneId; // Has to be set.
        public ushort InstanceId = 0;
        public uint CloneId; // Has to be set.
        public uint ZoneChecksum; // Has to be set.
        public byte EditorEnabled; // Has to be set.
        public byte EditorLevel; // Has to be set.
        public JVector Position; // Has to be set.
        public uint ZoneType = 0;

        public void FromBitstream(ReadOnlyBitStream packetStream)
        {
            throw new NotImplementedException();
        }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write(ZoneId);
            packetStream.Write(InstanceId);
            packetStream.Write(CloneId);
            packetStream.Write(ZoneChecksum);
            packetStream.Write(EditorEnabled);
            packetStream.Write(EditorLevel);
            packetStream.Write(Position.X);
            packetStream.Write(Position.Y);
            packetStream.Write(Position.Z);
            packetStream.Write(ZoneType);
        }
    }
}
