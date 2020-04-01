using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Enums
{
    public enum CharacterRenameResult : byte
    {
        Allow = 0,
        DenyInvalidCustomName = 2,
        DenyCustomNameTaken
    }
}
