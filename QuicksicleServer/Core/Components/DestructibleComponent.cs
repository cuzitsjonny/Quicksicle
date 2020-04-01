using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class DestructibleComponent : BaseReplicaComponent
    {
        public DestructibleComponent() : base(ReplicaComponentId.Destroyable) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            if (isInitialUpdate)
            {
                packetStream.Write0();
                packetStream.Write0();
            }
        }
    }
}
