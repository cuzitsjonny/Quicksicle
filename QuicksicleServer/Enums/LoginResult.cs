using System;

namespace Quicksicle.Enums
{
    public enum LoginResult : byte
    {
        DenyUnknownError,
        Allow,
        DenyCustomError = 5,
        DenyInvalidCredentials
    }
}
