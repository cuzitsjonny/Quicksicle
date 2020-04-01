using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class AuthLoginRequestPacket : IUnserializable
    {
        public string Username;
        public string Password;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            Username = packetStream.ReadWideString(33);
            Password = packetStream.ReadWideString(41);
        }
    }
}
