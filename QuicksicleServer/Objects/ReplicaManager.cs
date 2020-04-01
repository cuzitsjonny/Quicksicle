using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Quicksicle.Sessions;
using Quicksicle.IO;
using Quicksicle.Net;

namespace Quicksicle.Objects
{
    public class ReplicaManager
    {
        private ConcurrentQueue<OutgoingDatagramPacket> outgoingPacketQueue;
        private List<Session> playerSessions;
        private Dictionary<Replica, ushort> objects;
        private Dictionary<long, ushort> characterNetworkIdCache;
        private ushort nextNetworkId;

        public ReplicaManager(ConcurrentQueue<OutgoingDatagramPacket> outgoingPacketQueue)
        {
            this.outgoingPacketQueue = outgoingPacketQueue;
            this.playerSessions = new List<Session>();
            this.objects = new Dictionary<Replica, ushort>();
            this.characterNetworkIdCache = new Dictionary<long, ushort>();
            this.nextNetworkId = 1;
        }

        private ushort GenerateNetworkId()
        {
            ushort networkId;

            do
            {
                networkId = nextNetworkId++;
            } while (objects.ContainsValue(networkId) || networkId == 0);

            return networkId;
        }

        private void ConstructToPlayer(Replica replica, ushort networkId, Session playerSession)
        {
            WriteOnlyBitStream packetStream = new WriteOnlyBitStream();

            packetStream.Write((byte)36);
            packetStream.Write1();
            packetStream.Write(networkId);

            replica.Serialize(packetStream, true);

            outgoingPacketQueue.Enqueue(new OutgoingDatagramPacket(packetStream.ToByteArray(), playerSession.Address, playerSession.Port));
        }

        private void DestructToPlayer(ushort networkId, Session playerSession)
        {
            WriteOnlyBitStream packetStream = new WriteOnlyBitStream();

            packetStream.Write((byte)37);
            packetStream.Write(networkId);

            outgoingPacketQueue.Enqueue(new OutgoingDatagramPacket(packetStream.ToByteArray(), playerSession.Address, playerSession.Port));
        }

        private void SerializeToPlayer(Replica replica, ushort networkId, Session playerSession)
        {
            WriteOnlyBitStream packetStream = new WriteOnlyBitStream();

            packetStream.Write((byte)39);
            packetStream.Write(networkId);

            replica.Serialize(packetStream, false);

            outgoingPacketQueue.Enqueue(new OutgoingDatagramPacket(packetStream.ToByteArray(), playerSession.Address, playerSession.Port));
        }

        public void AddPlayer(Session playerSession, Replica character)
        {
            ushort characterNetworkId = 0;
            bool generateNetworkId = false;

            if (characterNetworkIdCache.ContainsKey(character.ObjectId))
            {
                characterNetworkId = characterNetworkIdCache[character.ObjectId];

                if (objects.ContainsValue(characterNetworkId))
                {
                    generateNetworkId = true;
                }
            }
            else
            {
                generateNetworkId = true;
            }

            if (generateNetworkId)
            {
                characterNetworkId = GenerateNetworkId();

                characterNetworkIdCache[character.ObjectId] = characterNetworkId;
            }

            Quicksicle.Core.Server.Instance.Logger.Log("Constructing " + character.Name + " with network ID " + characterNetworkId);

            foreach (Replica replica in objects.Keys)
            {
                ushort networkId = objects[replica];

                ConstructToPlayer(replica, networkId, playerSession);
            }

            ConstructToPlayer(character, characterNetworkId, playerSession);

            foreach (Session otherPlayerSession in playerSessions)
            {
                ConstructToPlayer(character, characterNetworkId, otherPlayerSession);
            }

            playerSession.ActiveCharacterReplica = character;

            objects[character] = characterNetworkId;
            playerSessions.Add(playerSession);
        }

        public void RemovePlayer(Session playerSession)
        {
            Replica replica = playerSession.ActiveCharacterReplica;

            if (replica != null)
            {
                ushort networkId = objects[replica];

                objects.Remove(replica);
                playerSessions.Remove(playerSession);

                foreach (Session otherPlayerSession in playerSessions)
                {
                    DestructToPlayer(networkId, otherPlayerSession);
                }

                DestructToPlayer(networkId, playerSession);

                playerSession.ActiveCharacterReplica = null;
            }
        }

        public void Update(Replica replica)
        {
            if (objects.ContainsKey(replica))
            {
                ushort networkId = objects[replica];

                foreach (Session otherPlayerSession in playerSessions)
                {
                    SerializeToPlayer(replica, networkId, otherPlayerSession);
                }
            }
        }
    }
}
