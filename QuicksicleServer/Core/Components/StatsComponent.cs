using System;
using System.Collections.Generic;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;

namespace Quicksicle.Core.Components
{
    public class StatsComponent : BaseReplicaComponent
    {
        public int ImmuneToBasicAttack = 0;
        public int ImmuneToDot = 0;
        public int ImmuneToKnockback = 0;
        public int ImmuneToInterrupt = 0;
        public int ImmuneToSpeed = 0;
        public int ImmuneToImaginationGain = 0;
        public int ImmuneToImaginationLoss = 0;
        public int ImmuneToQuickbuildInterrupt = 0;
        public int ImmuneToPullToPoint = 0;

        public int CurrentHealth; // Has to be set.
        public float MaxHealth; // Has to be set.
        public int CurrentArmor; // Has to be set.
        public float MaxArmor; // Has to be set.
        public int CurrentImagination; // Has to be set.
        public float MaxImagination; // Has to be set.
        public int DamageAbsorption = 0;

        public bool IsImmune = false;
        public bool IsGmImmune = false;
        public bool IsShielded = false;

        public List<int> FactionIds = new List<int>();  // Has to be modified.

        public bool IsSmashable;  // Has to be set.
        public bool IsDead = false;
        public bool IsSmashed = false;
        public bool IsModuleAssembly = false;
        public float ExplodeFactor = 1.0f;

        public List<long> EnemyThreats = new List<long>();

        public StatsComponent() : base(ReplicaComponentId.Stats) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            if (isInitialUpdate)
            {
                packetStream.Write1();
                packetStream.Write(ImmuneToBasicAttack);
                packetStream.Write(ImmuneToDot);
                packetStream.Write(ImmuneToKnockback);
                packetStream.Write(ImmuneToInterrupt);
                packetStream.Write(ImmuneToSpeed);
                packetStream.Write(ImmuneToImaginationGain);
                packetStream.Write(ImmuneToImaginationLoss);
                packetStream.Write(ImmuneToQuickbuildInterrupt);
                packetStream.Write(ImmuneToPullToPoint);
            }

            packetStream.Write1();
            packetStream.Write(CurrentHealth);
            packetStream.Write(MaxHealth);
            packetStream.Write(CurrentArmor);
            packetStream.Write(MaxArmor);
            packetStream.Write(CurrentImagination);
            packetStream.Write(MaxImagination);
            packetStream.Write(DamageAbsorption);

            packetStream.Write(IsImmune);
            packetStream.Write(IsGmImmune);
            packetStream.Write(IsShielded);

            packetStream.Write(MaxHealth);
            packetStream.Write(MaxArmor);
            packetStream.Write(MaxImagination);

            packetStream.Write(FactionIds.Count);

            for (int i = 0; i < FactionIds.Count; i++)
            {
                packetStream.Write(FactionIds[i]);
            }

            packetStream.Write(IsSmashable);

            if (isInitialUpdate)
            {
                packetStream.Write(IsDead);
                packetStream.Write(IsSmashed);

                if (IsSmashable)
                {
                    packetStream.Write(IsModuleAssembly);

                    if (ExplodeFactor != 1.0f)
                    {
                        packetStream.Write1();
                        packetStream.Write(ExplodeFactor);
                    }
                    else
                    {
                        packetStream.Write0();
                    }
                }
            }

            packetStream.Write1();
            packetStream.Write(EnemyThreats.Count > 0);
        }
    }
}
