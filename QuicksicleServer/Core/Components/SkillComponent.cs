using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class SkillComponent : BaseReplicaComponent
    {
        public SkillComponent() : base(ReplicaComponentId.Skill) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            if (isInitialUpdate)
            {
                packetStream.Write0();
            }
        }
    }
}
