using System;
using Quicksicle.IO;

namespace Quicksicle.Packets
{
    public class WorldCharacterCreateRequestPacket : IUnserializable
    {
        public string CustomName;
        public uint PredefinedNameFirst;
        public uint PredefinedNameMiddle;
        public uint PredefinedNameLast;
        public byte Unknown1 = 0;
        public uint HeadColor;
        public uint Head;
        public uint ChestColor;
        public uint Chest;
        public uint Legs;
        public uint HairStyle;
        public uint HairColor;
        public uint LeftHand;
        public uint RightHand;
        public uint EyebrowStyle;
        public uint EyesStyle;
        public uint MouthStyle;
        public byte Unknown2 = 0;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            CustomName = packetStream.ReadWideString(33);
            PredefinedNameFirst = packetStream.ReadUInt32();
            PredefinedNameMiddle = packetStream.ReadUInt32();
            PredefinedNameLast = packetStream.ReadUInt32();
            Unknown1 = packetStream.ReadByte();
            HeadColor = packetStream.ReadUInt32();
            Head = packetStream.ReadUInt32();
            ChestColor = packetStream.ReadUInt32();
            Chest = packetStream.ReadUInt32();
            Legs = packetStream.ReadUInt32();
            HairStyle = packetStream.ReadUInt32();
            HairColor = packetStream.ReadUInt32();
            LeftHand = packetStream.ReadUInt32();
            RightHand = packetStream.ReadUInt32();
            EyebrowStyle = packetStream.ReadUInt32();
            EyesStyle = packetStream.ReadUInt32();
            MouthStyle = packetStream.ReadUInt32();
            Unknown2 = packetStream.ReadByte();
        }
    }
}
