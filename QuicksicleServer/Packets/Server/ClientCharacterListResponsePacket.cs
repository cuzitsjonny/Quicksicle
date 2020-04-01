using System;
using System.Collections.Generic;
using Quicksicle.IO;
using Quicksicle.Database;

namespace Quicksicle.Packets
{
    public class ClientCharacterListResponsePacket : ISerializable
    {
        public class CharacterData
        {
            public uint ObjectTemplate = 1;
            public byte IsFtp = 0;
            public ushort Unknown1 = 0;
            public uint Unknown2 = 0;
            public ushort LastInstanceId = 0;
            public CharacterInfo CharacterInfo; // Has to be set
            public List<uint> EquippedObjectTemplates = new List<uint>();
        }

        public byte LastActiveCharacterIndex = 0;
        public List<CharacterData> CharacterDataList = new List<CharacterData>();

        public void FromBitstream(ReadOnlyBitStream packetStream)
        {
            throw new NotImplementedException();
        }

        public void ToBitStream(WriteOnlyBitStream packetStream)
        {
            packetStream.Write((byte)CharacterDataList.Count);
            packetStream.Write(LastActiveCharacterIndex);

            foreach (CharacterData characterData in CharacterDataList)
            {
                CharacterInfo characterInfo = characterData.CharacterInfo;

                packetStream.Write(characterInfo.CharacterId);
                packetStream.Write(characterData.ObjectTemplate);
                packetStream.WriteWideString(characterInfo.Name, 33);
                packetStream.WriteWideString(characterInfo.PendingName, 33);
                packetStream.Write(characterInfo.PendingNameRejected ? (byte)1 : (byte)0);
                packetStream.Write(characterData.IsFtp);
                packetStream.Write(characterData.Unknown1);
                packetStream.Write(characterInfo.HeadColor);
                packetStream.Write(characterInfo.Head);
                packetStream.Write(characterInfo.ChestColor);
                packetStream.Write(characterInfo.Chest);
                packetStream.Write(characterInfo.Legs);
                packetStream.Write(characterInfo.HairStyle);
                packetStream.Write(characterInfo.HairColor);
                packetStream.Write(characterInfo.LeftHand);
                packetStream.Write(characterInfo.RightHand);
                packetStream.Write(characterInfo.EyebrowStyle);
                packetStream.Write(characterInfo.EyesStyle);
                packetStream.Write(characterInfo.MouthStyle);
                packetStream.Write(characterData.Unknown2);
                packetStream.Write(characterInfo.ZoneId);
                packetStream.Write(characterData.LastInstanceId);
                packetStream.Write(characterInfo.CloneId);
                packetStream.Write(characterInfo.LastLogout);

                packetStream.Write((ushort)characterData.EquippedObjectTemplates.Count);

                foreach (uint equippedObjectTemplate in characterData.EquippedObjectTemplates)
                {
                    packetStream.Write(equippedObjectTemplate);
                }
            }
        }
    }
}
