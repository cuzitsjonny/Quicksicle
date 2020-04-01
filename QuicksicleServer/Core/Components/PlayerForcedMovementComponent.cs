using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class PlayerForcedMovementComponent : BaseReplicaComponent
    {
        public bool PlayerOnRail = false;
        public bool ShowBillboard = true;

        public PlayerForcedMovementComponent() : base(ReplicaComponentId.PlayerForcedMovement) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            packetStream.Write1();
            packetStream.Write(PlayerOnRail);
            packetStream.Write(ShowBillboard);
        }
    }
}
