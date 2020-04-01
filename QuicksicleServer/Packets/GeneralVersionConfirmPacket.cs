using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class GeneralVersionConfirmPacket : IUnserializable, ISerializable
    {
        public uint GameVersion;
        public uint Unknown1;
        public uint RemoteConnectionType;
        public uint ProcessId;
        public ushort LocalPort;
        public string LocalAddress;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            GameVersion = packetStream.ReadUInt32();
            Unknown1 = packetStream.ReadUInt32();
            RemoteConnectionType = packetStream.ReadUInt32();
            ProcessId = packetStream.ReadUInt32();
            LocalPort = packetStream.ReadUInt16();
            LocalAddress = packetStream.ReadString(33);
        }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write(GameVersion);
            packetStream.Write(Unknown1);
            packetStream.Write(RemoteConnectionType);
            packetStream.Write(ProcessId);
            packetStream.Write(LocalPort);
            packetStream.WriteString(LocalAddress, 33);
        }
    }
}
