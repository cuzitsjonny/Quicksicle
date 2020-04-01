using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Packets
{
    public abstract class BaseGameMessage
    {
        public long ObjectId;

        protected BaseGameMessage(long objectId)
        {
            this.ObjectId = objectId;
        }
    }
}
