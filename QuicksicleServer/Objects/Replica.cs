using System;
using System.Collections.Generic;
using Quicksicle.Enums;
using Quicksicle.IO;

namespace Quicksicle.Objects
{
    public class Replica
    {
        private List<BaseReplicaComponent> components;
        private List<long> childIds;

        public static readonly ReplicaComponentId[] SerializationOrder = { ReplicaComponentId.ControllablePhysics, ReplicaComponentId.Destroyable, ReplicaComponentId.Character, ReplicaComponentId.Skill, ReplicaComponentId.Render, ReplicaComponentId.Bbb };

        public Replica(long objectId, string configData, int template, long parentId)
        {
            this.components = new List<BaseReplicaComponent>();
            this.childIds = new List<long>();

            this.Name = String.Empty;
            this.ObjectId = objectId;
            this.Template = template;
            this.ParentId = parentId;
            this.UpdatePositionWithParent = false;
            this.Creation = DateTime.Now;
            this.SpawnerId = 0;
            this.SpawnerNodeId = -1;
            this.Scale = 1.0f;
            this.ObjectWorldState = 0;
            this.GmLevel = 0;
        }

        public long ObjectId
        {
            get;
        }

        public int Template
        {
            get;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime Creation
        {
            get;
        }

        public long SpawnerId
        {
            get;
        }

        public int SpawnerNodeId
        {
            get;
        }

        public float Scale
        {
            get;
        }

        public ObjectWorldState ObjectWorldState
        {
            get;
        }

        public byte GmLevel
        {
            get;
            set;
        }

        public long ParentId
        {
            get;
            set;
        }

        public bool UpdatePositionWithParent
        {
            get;
        }

        public long[] ChildIds
        {
            get { return childIds.ToArray(); }
        }

        public BaseReplicaComponent[] Components
        {
            get { return components.ToArray(); }
        }

        public void AddChildId(long childId)
        {
            childIds.Add(childId);
        }

        public void RemoveChildId(long childId)
        {
            childIds.Remove(childId);
        }

        public void AddComponent(BaseReplicaComponent component)
        {
            components.Add(component);
        }

        public void RemoveComponent(BaseReplicaComponent component)
        {
            components.Remove(component);
        }

        public bool HasComponentType(ReplicaComponentId componentId)
        {
            bool hasComponentType = false;

            foreach (BaseReplicaComponent component in components)
            {
                if (component.ComponentId == componentId)
                {
                    hasComponentType = true;
                }
            }

            return hasComponentType;
        }

        public BaseReplicaComponent GetComponent(ReplicaComponentId componentId)
        {
            BaseReplicaComponent c = null;

            foreach (BaseReplicaComponent component in components)
            {
                if (component.ComponentId == componentId)
                {
                    c = component;
                }
            }

            return c;
        }

        public void Serialize(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            if (isInitialUpdate)
            {
                packetStream.Write(ObjectId);
                packetStream.Write(Template);
                packetStream.Write((byte)Name.Length);

                if (Name.Length > 0)
                {
                    packetStream.WriteWideString(Name, Name.Length);
                }

                packetStream.Write((uint)DateTime.Now.Subtract(Creation).TotalSeconds);

                packetStream.Write0(); // config data
                packetStream.Write0(); // trigger id

                if (SpawnerId != 0)
                {
                    packetStream.Write1();
                    packetStream.Write(SpawnerId);
                }
                else
                {
                    packetStream.Write0();
                }

                if (SpawnerNodeId != -1)
                {
                    packetStream.Write1();
                    packetStream.Write(SpawnerNodeId);
                }
                else
                {
                    packetStream.Write0();
                }

                if (Scale != 1.0f)
                {
                    packetStream.Write1();
                    packetStream.Write(Scale);
                }
                else
                {
                    packetStream.Write0();
                }

                if (ObjectWorldState != ObjectWorldState.InWorld)
                {
                    packetStream.Write1();
                    packetStream.Write((byte)ObjectWorldState);
                }
                else
                {
                    packetStream.Write0();
                }

                if (GmLevel != 0)
                {
                    packetStream.Write1();
                    packetStream.Write(GmLevel);
                }
                else
                {
                    packetStream.Write0();
                }
            }

            packetStream.Write1();

            if (ParentId != 0)
            {
                packetStream.Write1();
                packetStream.Write(ParentId);
                packetStream.Write(UpdatePositionWithParent);
            }
            else
            {
                packetStream.Write0();
            }

            long[] childIds = ChildIds;

            if (childIds.Length > 0)
            {
                packetStream.Write1();
                packetStream.Write((ushort)childIds.Length);

                for (int i = 0; i < childIds.Length; i++)
                {
                    packetStream.Write(childIds[i]);
                }
            }
            else
            {
                packetStream.Write0();
            }

            bool statsComponentWritten = false;
            BaseReplicaComponent statsComponent = GetComponent(ReplicaComponentId.Stats);

            for (int i = 0; i < SerializationOrder.Length; i++)
            {
                ReplicaComponentId currentComponentId = SerializationOrder[i];

                foreach (BaseReplicaComponent component in components)
                {
                    if (component.ComponentId == currentComponentId)
                    {
                        if (!statsComponentWritten && statsComponent != null && (currentComponentId == ReplicaComponentId.COLLECTIBLE || currentComponentId == ReplicaComponentId.QUICKBUILD))
                        {
                            statsComponent.ToBitStream(packetStream, isInitialUpdate);
                            statsComponentWritten = true;
                        }

                        component.ToBitStream(packetStream, isInitialUpdate);

                        if (!statsComponentWritten && statsComponent != null && (currentComponentId == ReplicaComponentId.Destroyable))
                        {
                            statsComponent.ToBitStream(packetStream, isInitialUpdate);
                            statsComponentWritten = true;
                        }
                    }
                }
            }
        }
    }
}
