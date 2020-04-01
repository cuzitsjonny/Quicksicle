using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class WorldValidationPacket : IUnserializable
    {
        public string Username;
        public string SessionKey;
        public byte[] FdbChecksum;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            Username = packetStream.ReadWideString(33);
            SessionKey = packetStream.ReadWideString(33);
            FdbChecksum = packetStream.ReadBytes(33);
        }
    }
}
