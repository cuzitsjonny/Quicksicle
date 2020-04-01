using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;
using Jitter.LinearMath;

namespace Quicksicle.Core.Components
{
    public class ControllablePhysicsComponent : BasePhysicsComponent
    {
        public bool IsInJetPackMode = false;
        public int JetPackEffectId = 0;
        public bool JetPackFlying = false;
        public bool JetPackBypassChecks = false;

        public int ImmuneToStunMove = 0;
        public int ImmuneToStunJump = 0;
        public int ImmuneToStunTurn = 0;
        public int ImmuneToStunAttack = 0;
        public int ImmuneToStunUseItem = 0;
        public int ImmuneToStunEquip = 0;
        public int ImmuneToStunInteract = 0;

        public ControllablePhysicsComponent() : base(ReplicaComponentId.ControllablePhysics) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            if (isInitialUpdate)
            {
                if (IsInJetPackMode)
                {
                    packetStream.Write1();
                    packetStream.Write(JetPackEffectId);
                    packetStream.Write(JetPackFlying);
                    packetStream.Write(JetPackBypassChecks);
                }
                else
                {
                    packetStream.Write0();
                }

                packetStream.Write1();
                packetStream.Write(ImmuneToStunMove);
                packetStream.Write(ImmuneToStunJump);
                packetStream.Write(ImmuneToStunTurn);
                packetStream.Write(ImmuneToStunAttack);
                packetStream.Write(ImmuneToStunUseItem);
                packetStream.Write(ImmuneToStunEquip);
                packetStream.Write(ImmuneToStunInteract);
            }

            packetStream.Write(false);
            packetStream.Write(false);
            packetStream.Write(false);

            base.ToBitStream(packetStream, isInitialUpdate);
        }
    }
}
