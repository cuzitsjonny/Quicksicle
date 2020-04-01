using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;
using Jitter.LinearMath;

namespace Quicksicle.Core.Components
{
    public class BasePhysicsComponent : BaseReplicaComponent
    {
        public JVector Position; // Has to be set.
        public JQuaternion Rotation; // Has to be set.
        public bool IsSupported = true;
        public bool IsOnRail = false;

        public JVector LinearVelocity = JVector.Zero;
        public JVector AngularVelocity = JVector.Zero;

        public long LocalSpaceObjectId = 0;
        public JVector LocalPosition = JVector.Zero;
        public JVector LocalLinearVelocity = JVector.Zero;

        public bool Teleport = false;

        public BasePhysicsComponent(ReplicaComponentId componentId) : base(componentId) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            packetStream.Write1();
            packetStream.Write(Position.X);
            packetStream.Write(Position.Y);
            packetStream.Write(Position.Z);
            packetStream.Write(Rotation.X);
            packetStream.Write(Rotation.Y);
            packetStream.Write(Rotation.Z);
            packetStream.Write(Rotation.W);
            packetStream.Write(IsSupported);
            packetStream.Write(IsOnRail);

            if (LinearVelocity.LengthSquared() > JMath.Epsilon)
            {
                packetStream.Write1();
                packetStream.Write(LinearVelocity.X);
                packetStream.Write(LinearVelocity.Y);
                packetStream.Write(LinearVelocity.Z);
            }
            else
            {
                packetStream.Write0();
            }

            if (AngularVelocity.LengthSquared() > JMath.Epsilon)
            {
                packetStream.Write1();
                packetStream.Write(AngularVelocity.X);
                packetStream.Write(AngularVelocity.Y);
                packetStream.Write(AngularVelocity.Z);
            }
            else
            {
                packetStream.Write0();
            }

            if (LocalSpaceObjectId != 0)
            {
                packetStream.Write1();
                packetStream.Write(LocalSpaceObjectId);
                packetStream.Write(LocalPosition.X);
                packetStream.Write(LocalPosition.Y);
                packetStream.Write(LocalPosition.Z);

                if (LocalLinearVelocity.LengthSquared() > JMath.Epsilon)
                {
                    packetStream.Write1();
                    packetStream.Write(LocalLinearVelocity.X);
                    packetStream.Write(LocalLinearVelocity.Y);
                    packetStream.Write(LocalLinearVelocity.Z);
                }
                else
                {
                    packetStream.Write0();
                }
            }
            else
            {
                packetStream.Write0();
            }

            if (!isInitialUpdate)
            {
                packetStream.Write(Teleport);
            }
        }
    }
}
