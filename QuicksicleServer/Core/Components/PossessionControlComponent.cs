using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class PossessionControlComponent : BaseReplicaComponent
    {
        public long PossessedObject = 0;
        public PossessionType PossessionType = PossessionType.NoPossession;

        public PossessionControlComponent() : base(ReplicaComponentId.PossessionControl) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            packetStream.Write1();

            if (PossessedObject != 0)
            {
                packetStream.Write1();
                packetStream.Write(PossessedObject);
            }
            else
            {
                packetStream.Write0();
            }

            packetStream.Write((byte)PossessionType);
        }
    }
}
