using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Enums
{
    public enum DisconnectReason : uint
    {
        NoReason,
        InvalidGameVersion,
        InvalidServerVersion,
        DuplicateLogin = 4,
        ServerShutdown,
        MapCorruption,
        InvalidSessionKey,
        UnauthenticatedAccount,
        CharacterNotFound,
        CharacterCorruption,
        Kick
    }
}
