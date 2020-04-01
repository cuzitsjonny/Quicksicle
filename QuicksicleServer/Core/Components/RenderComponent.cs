using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class RenderComponent : BaseReplicaComponent
    {
        public RenderComponent() : base(ReplicaComponentId.Render) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            if(isInitialUpdate)
            {
                packetStream.Write(0);
            }
        }
    }
}
