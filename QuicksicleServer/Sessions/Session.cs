using System;
using System.Security.Cryptography;
using Quicksicle.IO;
using Quicksicle.Database;
using Quicksicle.Objects;

namespace Quicksicle.Sessions
{
    public class Session
    {
        public Session(string address, ushort port, AccountInfo activeAccountInfo)
        {
            this.Address = address;
            this.Port = port;
            this.Secret = GenerateSecret();
            this.ActiveAccountInfo = activeAccountInfo;
        }

        private string GenerateSecret()
        {
            byte[] bytes = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            rng.GetBytes(bytes);

            return IOUtils.ConvertBin2Hex(bytes);
        }

        public string Address
        {
            get;
            set;
        }

        public ushort Port
        {
            get;
            set;
        }

        public string Secret
        {
            get;
        }

        public AccountInfo ActiveAccountInfo
        {
            get;
        }

        public CharacterInfo ActiveCharacterInfo
        {
            get;
            set;
        }

        public Replica ActiveCharacterReplica
        {
            get;
            set;
        }
    }
}
