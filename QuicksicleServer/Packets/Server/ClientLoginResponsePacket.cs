using System;
using Quicksicle.IO;
using Quicksicle.Enums;

namespace Quicksicle.Packets
{
    public class ClientLoginResponsePacket : ISerializable
    {
        public LoginResult LoginResult; // Has to be set
        public string Unknown1 = "Talk_Like_A_Pirate";
        public string Unknown2 = String.Empty;
        public string Unknown3 = String.Empty;
        public string Unknown4 = String.Empty;
        public string Unknown5 = String.Empty;
        public string Unknown6 = String.Empty;
        public string Unknown7 = String.Empty;
        public string Unknown8 = String.Empty;
        public ushort ClientVersionMajor = 1;
        public ushort ClientVersionCurrent = 10;
        public ushort ClientVersionMinor = 64;
        public string SessionSecret; // Has to be set
        public string CharacterInstanceIp; // Has to be set
        public string ChatInstanceIp; // Has to be set
        public ushort CharacterInstancePort; // Has to be set
        public ushort ChatInstancePort; // Has to be set
        public string Unknown9 = String.Empty;
        public string UniqueId = "00000000-0000-0000-0000-000000000000";
        public uint Unknown10 = 0;
        public string Locale = "US";
        public byte JustSubscribed = 0;
        public byte IsFtp = 0;
        public ulong Unknown11 = 0;
        public string CustomErrorMessage; // Has to be set

        public void FromBitstream(ReadOnlyBitStream packetStream)
        {
            throw new NotImplementedException();
        }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write((byte)LoginResult);
            packetStream.WriteString(Unknown1, 33);
            packetStream.WriteString(Unknown2, 33);
            packetStream.WriteString(Unknown3, 33);
            packetStream.WriteString(Unknown4, 33);
            packetStream.WriteString(Unknown5, 33);
            packetStream.WriteString(Unknown6, 33);
            packetStream.WriteString(Unknown7, 33);
            packetStream.WriteString(Unknown8, 33);
            packetStream.Write(ClientVersionMajor);
            packetStream.Write(ClientVersionCurrent);
            packetStream.Write(ClientVersionMinor);
            packetStream.WriteWideString(SessionSecret, 33);
            packetStream.WriteString(CharacterInstanceIp, 33);
            packetStream.WriteString(ChatInstanceIp, 33);
            packetStream.Write(CharacterInstancePort);
            packetStream.Write(ChatInstancePort);
            packetStream.WriteString(Unknown9, 33);
            packetStream.WriteString(UniqueId, 37);
            packetStream.Write(Unknown10);
            packetStream.WriteString(Locale, 3);
            packetStream.Write(JustSubscribed);
            packetStream.Write(IsFtp);
            packetStream.Write(Unknown11);

            if (CustomErrorMessage != null)
            {
                packetStream.Write((ushort)CustomErrorMessage.Length);
                packetStream.WriteWideString(CustomErrorMessage, CustomErrorMessage.Length);
            }
            else
            {
                packetStream.Write((ushort)0);
            }

            packetStream.Write((uint)4);
        }
    }
}
