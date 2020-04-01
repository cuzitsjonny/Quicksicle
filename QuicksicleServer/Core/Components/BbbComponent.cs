using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class BbbComponent : BaseReplicaComponent
    {
        public long MetadataSourceItem = 0;

        public BbbComponent() : base(ReplicaComponentId.Bbb) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            packetStream.Write1();
            packetStream.Write(MetadataSourceItem);
        }
    }
}
