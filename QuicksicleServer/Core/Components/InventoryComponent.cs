using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class InventoryComponent : BaseReplicaComponent
    {
        public InventoryComponent() : base(ReplicaComponentId.Inventory) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            packetStream.Write0();
            packetStream.Write0();
        }
    }
}
