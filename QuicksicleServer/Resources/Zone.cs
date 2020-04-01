using System;
using System.IO;
using System.Collections.Generic;
using Jitter.LinearMath;
using Quicksicle.IO;

namespace Quicksicle.Resources
{
    public class Zone
    {
        private List<SceneInfo> sceneInfos;

        public Zone(string filePath)
        {
            this.sceneInfos = new List<SceneInfo>();

            ReadOnlyBitStream reader = new ReadOnlyBitStream(File.ReadAllBytes(filePath));

            this.Version = reader.ReadUInt32();

            if (this.Version >= 0x24)
            {
                reader.SkipBytes(4);
            }

            this.WorldId = reader.ReadUInt32();

            if (this.Version >= 0x26)
            {
                float spawnPosX = reader.ReadSingle();
                float spawnPosY = reader.ReadSingle();
                float spawnPosZ = reader.ReadSingle();

                this.SpawnPosition = new JVector(spawnPosX, spawnPosY, spawnPosZ);

                float spawnRotX = reader.ReadSingle();
                float spawnRotY = reader.ReadSingle();
                float spawnRotZ = reader.ReadSingle();
                float spawnRotW = reader.ReadSingle();

                this.SpawnRotation = new JQuaternion(spawnRotX, spawnRotY, spawnRotZ, spawnRotW);
            }
            else
            {
                this.SpawnPosition = JVector.Zero;
                this.SpawnRotation = new JQuaternion(0.0f, 0.0f, 0.0f, 0.0f);
            }

            uint sceneCount;

            if (this.Version >= 0x25)
            {
                sceneCount = reader.ReadUInt32();
            }
            else
            {
                sceneCount = reader.ReadByte();
            }

            for (uint i = 0; i < sceneCount; i++)
            {
                byte sceneFileNameLength = reader.ReadByte();
                string sceneFileName = String.Empty;

                for (byte b = 0; b < sceneFileNameLength; b++)
                {
                    byte c = reader.ReadByte();

                    sceneFileName += (char)c;
                }

                reader.SkipBytes(1);
                reader.SkipBytes(3);
                reader.SkipBytes(1);
                reader.SkipBytes(3);

                byte sceneNameLength = reader.ReadByte();
                string sceneName = String.Empty;

                for (byte b = 0; b < sceneNameLength; b++)
                {
                    byte c = reader.ReadByte();

                    sceneName += (char)c;
                }

                sceneInfos.Add(new SceneInfo(sceneName, sceneFileName));
            }

            reader.SkipBytes(1);

            byte length = reader.ReadByte();

            for (byte b = 0; b < length; b++)
            {
                reader.SkipBytes(1);
            }

            length = reader.ReadByte();

            for (byte b = 0; b < length; b++)
            {
                reader.SkipBytes(1);
            }

            length = reader.ReadByte();

            for (byte b = 0; b < length; b++)
            {
                reader.SkipBytes(1);
            }

            // SCENE TRANSITIONS
        }

        public uint Version
        {
            get;
        }

        public uint WorldId
        {
            get;
        }

        public JVector SpawnPosition
        {
            get;
        }

        public JQuaternion SpawnRotation
        {
            get;
        }

        public SceneInfo[] SceneInfos
        {
            get { return sceneInfos.ToArray(); }
        }
    }
}
