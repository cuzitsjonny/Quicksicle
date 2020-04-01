using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Quicksicle.Logging;
using Quicksicle.Tasks;
using Quicksicle.Net;
using Quicksicle.IO;
using Quicksicle.Events;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Resources;
using Quicksicle.Enums;
using Quicksicle.Objects;

using Quicksicle.Core.Events;
using Quicksicle.Core.Subscribers;

namespace Quicksicle.Core
{
    /// <summary>
    /// The actual server of Quicksicle.
    /// Here packets are processed, scheduling is done, and all
    /// of that interesting stuff.
    /// </summary>
    public class Server
    {
        // A static instance of this object;
        private static Server instance;

        // The UDP ports thi server instance is bound to.
        private ushort authPort;
        private ushort worldPort;

        // The logger already know from QuicksicleBridge.
        private Logger logger;

        // The database connection manager.
        private DatabaseManager databaseManager;

        // The object id generator.
        private ObjectIdGenerator objectIdGenerator;

        // A cache for predefined character names.
        private PredefinedNameCache predefinedNameCache;

        // A cache for all zone checksums.
        private ZoneChecksumCache zoneChecksumCache;

        // The incoming packet queue of the server.
        // Packets in here will be processed in the packet processor.
        private ConcurrentQueue<IncomingDatagramPacket> incomingPacketQueue;

        // The outgoing packet queue of the server.
        // Packets in here will be sent using RakNet's peer.
        private ConcurrentQueue<OutgoingDatagramPacket> outgoingPacketQueue;

        // The scheduler of the server. Every action in the packet processor should be
        // scheduled using this. It supports synchronous and asynchronous operations.
        private Scheduler scheduler;

        // The event manager. Sometimes it is easier to use this instead of the scheduler,
        // since scheduled actions need to be chained for e.g. an async wait.
        // It uses the publish/subscribe pattern and can handle synchronous and asynchronous
        // events/callbacks. It internally uses the scheduler to run callbacks.
        private EventManager eventManager;

        // The session manager. Here all the information about an authenticated user can be found.
        private SessionManager sessionManager;

        // The packet processor.
        private PacketProcessor packetProcessor;

        // Each world has an own replica manager.
        private Dictionary<ushort, ReplicaManager> replicaManagers;

        // LUZ files in their real form.
        private List<Zone> zones;

        /// <summary>
        /// Constructs a new server object and binds it to the
        /// class level instance variable.
        /// The only object that should ever exist of this class
        /// is constructed in QuicksicleBridge.
        /// </summary>
        public Server(ushort authPort, ushort worldPort, Logger logger, DatabaseManager databaseConnectionManager, PredefinedNameCache predefinedNameCache, List<Zone> zones)
        {
            this.authPort = authPort;
            this.worldPort = worldPort;
            this.logger = logger;
            this.databaseManager = databaseConnectionManager;
            this.predefinedNameCache = predefinedNameCache;
            this.zoneChecksumCache = new ZoneChecksumCache();
            this.incomingPacketQueue = new ConcurrentQueue<IncomingDatagramPacket>();
            this.outgoingPacketQueue = new ConcurrentQueue<OutgoingDatagramPacket>();
            this.scheduler = new Scheduler();
            this.eventManager = new EventManager();
            this.sessionManager = new SessionManager();
            this.packetProcessor = new PacketProcessor(incomingPacketQueue);
            this.replicaManagers = new Dictionary<ushort, ReplicaManager>();
            this.zones = zones;

            instance = this;

            InitializeSubscribers();
        }

        /// <summary>
        /// Here we have Quicksicle's initial subscribers.
        /// These are the only subscribers initialized and
        /// triggered by the server. All others are either
        /// initialized in the ServerStartSubscriber's
        /// OnServerStart method, or somewhere else
        /// during runtime.
        /// They are all triggered on the scheduler
        /// main thread. So the basic server structure
        /// allows synchronous-only and asynchronous
        /// implementations.
        /// </summary>
        private void InitializeSubscribers()
        {
            new ServerStartSubscriber();
            new ServerShutdownSubscriber();
            new ClientConnectSubscriber();
            new ClientDisconnectSubscriber();
        }

        /// <summary>
        /// Getter for the static instance member.
        /// This is used to access the server
        /// object anywhere in the code.
        /// </summary>
        public static Server Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Getter for the member authPort.
        /// </summary>
        public ushort AuthPort
        {
            get { return authPort; }
        }

        /// <summary>
        /// Getter for the member worldPort.
        /// </summary>
        public ushort WorldPort
        {
            get { return worldPort; }
        }

        /// <summary>
        /// Getter for the member logger.
        /// </summary>
        public Logger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Getter for the member databaseManager.
        /// </summary>
        public DatabaseManager DatabaseManager
        {
            get { return databaseManager; }
        }

        /// <summary>
        /// Getter for the member objectIdGenerator.
        /// </summary>
        public ObjectIdGenerator ObjectIdGenerator
        {
            get { return objectIdGenerator; }
        }

        /// <summary>
        /// Getter for the member predefinedNameCache.
        /// </summary>
        public PredefinedNameCache PredefinedNameCache
        {
            get { return predefinedNameCache; }
        }

        /// <summary>
        /// Getter for the member zoneChecksumCache.
        /// </summary>
        public ZoneChecksumCache ZoneChecksumCache
        {
            get { return zoneChecksumCache; }
        }

        /// <summary>
        /// Getter for the member incomingPacketQueue.
        /// </summary>
        public ConcurrentQueue<IncomingDatagramPacket> IncomingPacketQueue
        {
            get { return incomingPacketQueue; }
        }

        /// <summary>
        /// Getter for the member outgoingPacketQueue.
        /// </summary>
        public ConcurrentQueue<OutgoingDatagramPacket> OutgoingPacketQueue
        {
            get { return outgoingPacketQueue; }
        }

        /// <summary>
        /// Getter for the member scheduler.
        /// </summary>
        public Scheduler Scheduler
        {
            get { return scheduler; }
        }

        /// <summary>
        /// Getter for the member eventManager.
        /// </summary>
        public EventManager EventManager
        {
            get { return eventManager; }
        }

        /// <summary>
        /// Getter for the member sessionManager.
        /// </summary>
        public SessionManager SessionManager
        {
            get { return sessionManager; }
        }

        public void SendGamePacket(ISerializable gamePacket, RemoteConnectionType remoteConnectionType, uint packetId, string destinationAddress, ushort destinationPort)
        {
            WriteOnlyBitStream packetStream = new WriteOnlyBitStream();

            packetStream.Write((byte)83);
            packetStream.Write((ushort)remoteConnectionType);
            packetStream.Write(packetId);
            packetStream.Write((byte)0);

            gamePacket.ToBitStream(packetStream);

            OutgoingDatagramPacket packet = new OutgoingDatagramPacket(packetStream.ToByteArray(), destinationAddress, destinationPort);

            outgoingPacketQueue.Enqueue(packet);
        }

        public void SendGamePacket(ISerializable gamePacket, GeneralPacketId packetId, string destinationAddress, ushort destinationPort)
        {
            SendGamePacket(gamePacket, RemoteConnectionType.General, (uint)packetId, destinationAddress, destinationPort);
        }

        public void SendGamePacket(ISerializable gamePacket, AuthPacketId packetId, string destinationAddress, ushort destinationPort)
        {
            SendGamePacket(gamePacket, RemoteConnectionType.Auth, (uint)packetId, destinationAddress, destinationPort);
        }

        public void SendGamePacket(ISerializable gamePacket, ChatPacketId packetId, string destinationAddress, ushort destinationPort)
        {
            SendGamePacket(gamePacket, RemoteConnectionType.Chat, (uint)packetId, destinationAddress, destinationPort);
        }

        public void SendGamePacket(ISerializable gamePacket, ClientPacketId packetId, string destinationAddress, ushort destinationPort)
        {
            SendGamePacket(gamePacket, RemoteConnectionType.Client, (uint)packetId, destinationAddress, destinationPort);
        }

        public void SendGamePacket(ISerializable gamePacket, WorldPacketId packetId, string destinationAddress, ushort destinationPort)
        {
            SendGamePacket(gamePacket, RemoteConnectionType.World, (uint)packetId, destinationAddress, destinationPort);
        }

        public ReplicaManager GetReplicaManager(ushort zoneId)
        {
            ReplicaManager replicaManager;

            if (replicaManagers.ContainsKey(zoneId))
            {
                replicaManager = replicaManagers[zoneId];
            }
            else
            {
                replicaManager = new ReplicaManager(outgoingPacketQueue);

                replicaManagers[zoneId] = replicaManager;
            }

            return replicaManager;
        }

        public Zone GetZone(ushort zoneId)
        {
            Zone zone = null;

            foreach (Zone z in zones)
            {
                if (z.WorldId == zoneId)
                {
                    zone = z;
                }
            }

            return zone;
        }

        /// <summary>
        /// Starts all members.
        /// </summary>
        public void Start()
        {
            databaseManager.Start();
            scheduler.Start();
            packetProcessor.Start();

            MySqlHandle mySqlHandle = databaseManager.GetMySqlHandle();

            try
            {
                mySqlHandle.Open();

                List<long> takenObjectIds = new List<long>();

                mySqlHandle.CharactersGetCharacterIds(takenObjectIds);

                objectIdGenerator = new ObjectIdGenerator(takenObjectIds);

                mySqlHandle.Close();
            }
            catch (Exception exc)
            {
                objectIdGenerator = new ObjectIdGenerator();

                LogDatabaseError(exc);
            }

            mySqlHandle.Free();

            eventManager.Publish(new ServerStartEvent());
        }

        /// <summary>
        /// Shuts down all members.
        /// </summary>
        public void ShutDown()
        {
            eventManager.Publish(new ServerShutdownEvent());

            packetProcessor.Terminate();
            scheduler.Terminate();
            databaseManager.ShutDown();
        }

        public void LogDatabaseError(Exception exc)
        {
            Server.Instance.Logger.Log("A database error has occured. exception=" + exc.Message);
        }
    }
}
