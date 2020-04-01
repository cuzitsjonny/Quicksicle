using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class LevelProgressionComponent : BaseReplicaComponent
    {
        public uint Level = 1;

        public LevelProgressionComponent() : base(ReplicaComponentId.LevelProgression) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            packetStream.Write1();
            packetStream.Write(Level);
        }
    }
}
