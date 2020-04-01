using System;
using System.Collections.Generic;
using System.Threading;
using Quicksicle.Enums;

namespace Quicksicle.Objects
{
    public class ObjectIdGenerator
    {
        private List<long> takenIds;
        private long nextGlobalId;
        private long nextSpawnedId;

        public ObjectIdGenerator()
        {
            this.takenIds = new List<long>();
            this.nextGlobalId = 1;
            this.nextSpawnedId = 1;
        }

        public ObjectIdGenerator(List<long> takenObjectIds)
        {
            this.takenIds = takenObjectIds;
            this.nextGlobalId = 1;
            this.nextSpawnedId = 1;
        }

        private bool IsTaken(long objectId)
        {
            return takenIds.Contains(objectId);
        }

        public static ObjectIdType GetObjectIdType(long objectId)
        {
            if ((objectId & (1L << 58)) != 0)
            {
                return ObjectIdType.Spawned;
            }
            else if ((objectId & (31L << 59)) != 0)
            {
                return ObjectIdType.Global;
            }
            else if ((objectId & (1L << 58)) != 0 && (objectId & (1L << 46)) != 0)
            {
                return ObjectIdType.Local;
            }
            else if ((objectId & (63L << 58)) == 0)
            {
                return ObjectIdType.Static;
            }
            else
            {
                return ObjectIdType.Invalid;
            }
        }

        public long GenerateGlobalId()
        {
            Monitor.Enter(takenIds);

            long id;
            long globalId;

            do
            {
                id = nextGlobalId++;
                globalId = id | (1L << 60);
            } while (id == globalId);

            if (IsTaken(globalId))
            {
                globalId = GenerateGlobalId();
            }
            else
            {
                takenIds.Add(globalId);
            }

            Monitor.Exit(takenIds);

            return globalId;
        }

        public long GenerateSpawnedId()
        {
            Monitor.Enter(takenIds);

            long id;
            long spawnedId;

            do
            {
                id = nextSpawnedId++;
                spawnedId = id | (1L << 58);
            } while (id == spawnedId);

            if (IsTaken(spawnedId))
            {
                spawnedId = GenerateSpawnedId();
            }
            else
            {
                takenIds.Add(spawnedId);
            }

            Monitor.Exit(takenIds);

            return spawnedId;
        }

        public void FreeObjectId(long objectId)
        {
            Monitor.Enter(takenIds);

            takenIds.Remove(objectId);

            Monitor.Exit(takenIds);
        }
    }
}
