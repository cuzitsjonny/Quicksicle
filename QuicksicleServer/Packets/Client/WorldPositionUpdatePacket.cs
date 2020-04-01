using System;
using Quicksicle.IO;
using Jitter.LinearMath;

namespace Quicksicle.Packets
{
    public class WorldPositionUpdatePacket : IUnserializable
    {
        public JVector Position;
        public JQuaternion Rotation;
        public bool IsSupported;
        public bool IsOnRail;

        public JVector LinearVelocity = JVector.Zero;
        public JVector AngularVelocity = JVector.Zero;

        public long LocalSpaceObjectId = 0;
        public JVector LocalPosition = JVector.Zero;
        public JVector LocalLinearVelocity = JVector.Zero;

        public void FromBitStream(ReadOnlyBitStream packetStream)
        {
            float positionX = packetStream.ReadSingle();
            float positionY = packetStream.ReadSingle();
            float positionZ = packetStream.ReadSingle();

            Position = new JVector(positionX, positionY, positionZ);

            float rotationX = packetStream.ReadSingle();
            float rotationY = packetStream.ReadSingle();
            float rotationZ = packetStream.ReadSingle();
            float rotationW = packetStream.ReadSingle();

            Rotation = new JQuaternion(rotationX, rotationY, rotationZ, rotationW);

            IsSupported = packetStream.ReadBit();
            IsOnRail = packetStream.ReadBit();

            bool flag = packetStream.ReadBit();

            if (flag)
            {
                float linearVelocityX = packetStream.ReadSingle();
                float linearVelocityY = packetStream.ReadSingle();
                float linearVelocityZ = packetStream.ReadSingle();

                LinearVelocity = new JVector(linearVelocityX, linearVelocityY, linearVelocityZ);
            }

            flag = packetStream.ReadBit();

            if (flag)
            {
                float angularVelocityX = packetStream.ReadSingle();
                float angularVelocityY = packetStream.ReadSingle();
                float angularVelocityZ = packetStream.ReadSingle();

                AngularVelocity = new JVector(angularVelocityX, angularVelocityY, angularVelocityZ);
            }

            flag = packetStream.ReadBit();

            if (flag)
            {
                LocalSpaceObjectId = packetStream.ReadInt64();

                float localPositionX = packetStream.ReadSingle();
                float localPositionY = packetStream.ReadSingle();
                float localPositionZ = packetStream.ReadSingle();

                LocalPosition = new JVector(localPositionX, localPositionY, localPositionZ);

                flag = packetStream.ReadBit();

                if (flag)
                {
                    float localLinearVelocityX = packetStream.ReadSingle();
                    float localLinearVelocityY = packetStream.ReadSingle();
                    float localLinearVelocityZ = packetStream.ReadSingle();

                    LocalLinearVelocity = new JVector(localLinearVelocityX, localLinearVelocityY, localLinearVelocityZ);
                }
            }
        }
    }
}
