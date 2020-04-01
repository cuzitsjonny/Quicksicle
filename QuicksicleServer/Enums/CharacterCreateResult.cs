using System;

namespace Quicksicle.Enums
{
    public enum CharacterCreateResult : byte
    {
        Allow,
        DenyInvalidCustomName = 2,
        DenyPredefinedNameTaken,
        DenyCustomNameTaken
    }
}
