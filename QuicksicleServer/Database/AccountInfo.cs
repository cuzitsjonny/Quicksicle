using System;
using Quicksicle.IO;

namespace Quicksicle.Database
{
    public class AccountInfo
    {
        public AccountInfo(long accountId, string username, string passwordSaltHex, string passwordHashHex, int passwordIterations)
        {
            this.AccountId = accountId;
            this.Username = username;
            this.PasswordSalt = IOUtils.ConvertHex2Bin(passwordSaltHex);
            this.PasswordHash = IOUtils.ConvertHex2Bin(passwordHashHex);
            this.PasswordIterations = passwordIterations;
        }

        public long AccountId
        {
            get;
        }

        public string Username
        {
            get;
        }

        public byte[] PasswordSalt
        {
            get;
        }

        public byte[] PasswordHash
        {
            get;
        }

        public int PasswordIterations
        {
            get;
        }
    }
}
