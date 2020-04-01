using System;
using Jitter.LinearMath;

namespace Quicksicle.Database
{
    public class CharacterInfo
    {
        public CharacterInfo(long characterId, long accountId)
        {
            this.CharacterId = characterId;
            this.AccountId = accountId;
        }

        public long CharacterId
        {
            get;
        }

        public long AccountId
        {
            get;
        }

        public string Name
        {
            get;
            set;
        }

        public string PendingName
        {
            get;
            set;
        }

        public bool PendingNameRejected
        {
            get;
            set;
        }

        public uint HeadColor
        {
            get;
            set;
        }

        public uint Head
        {
            get;
            set;
        }

        public uint ChestColor
        {
            get;
            set;
        }

        public uint Chest
        {
            get;
            set;
        }

        public uint Legs
        {
            get;
            set;
        }

        public uint HairStyle
        {
            get;
            set;
        }

        public uint HairColor
        {
            get;
            set;
        }

        public uint LeftHand
        {
            get;
            set;
        }

        public uint RightHand
        {
            get;
            set;
        }

        public uint EyebrowStyle
        {
            get;
            set;
        }

        public uint EyesStyle
        {
            get;
            set;
        }

        public uint MouthStyle
        {
            get;
            set;
        }

        public ushort ZoneId
        {
            get;
            set;
        }

        public uint CloneId
        {
            get;
            set;
        }

        public ulong LastLogout
        {
            get;
            set;
        }

        public JVector Position
        {
            get;
            set;
        }

        public JQuaternion Rotation
        {
            get;
            set;
        }

        public ulong UniverseScore
        {
            get;
            set;
        }

        public byte GmLevel
        {
            get;
            set;
        }

        public byte EditorLevel
        {
            get;
            set;
        }

        public byte CurrentHealth
        {
            get;
            set;
        }

        public byte MaxHealth
        {
            get;
            set;
        }
        public byte CurrentArmor
        {
            get;
            set;
        }
        public byte MaxArmor
        {
            get;
            set;
        }
        public byte CurrentImagination
        {
            get;
            set;
        }
        public byte MaxImagination
        {
            get;
            set;
        }
    }
}
