using System;
using Quicksicle.Enums;
using Quicksicle.IO;

namespace Quicksicle.Objects
{
    public abstract class BaseReplicaComponent
    {
        protected BaseReplicaComponent(ReplicaComponentId componentId)
        {
            this.ComponentId = componentId;
        }

        public ReplicaComponentId ComponentId
        {
            get;
        }

        public abstract void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate);
    }
}
