using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using Quicksicle.Threading;
using Quicksicle.Net;
using Quicksicle.IO;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core
{
    public class PacketProcessor : BaseThread
    {
        private ConcurrentQueue<IncomingDatagramPacket> incomingPacketQueue;

        public PacketProcessor(ConcurrentQueue<IncomingDatagramPacket> incomingPacketQueue)
        {
            this.incomingPacketQueue = incomingPacketQueue;
        }

        public override void Run()
        {
            while (!terminationRequested || !incomingPacketQueue.IsEmpty)
            {
                IncomingDatagramPacket packet;

                if (incomingPacketQueue.TryDequeue(out packet))
                {
                    ReadOnlyBitStream packetStream = new ReadOnlyBitStream(packet.Data);

                    byte rakNetPacketId = packetStream.ReadByte();

                    switch (rakNetPacketId)
                    {
                        case 17: // ID_NEW_INCOMING_CONNECTION
                            {
                                Server.Instance.Scheduler.RunTask(() => Server.Instance.EventManager.Publish(new ClientConnectEvent(packet.SourceAddress, packet.SourcePort, packet.DestinationAddress, packet.DestinationPort)));
                                break;
                            }

                        case 19: // ID_DISCONNECTION_NOTIFICATION
                        case 20: // ID_CONNECTION_LOST
                            {
                                Server.Instance.Scheduler.RunTask(() => Server.Instance.EventManager.Publish(new ClientDisconnectEvent(packet.SourceAddress, packet.SourcePort, packet.DestinationAddress, packet.DestinationPort)));
                                break;
                            }

                        case 83: // ID_USER_PACKET_ENUM
                            {
                                try
                                {
                                    RemoteConnectionType remoteConnectionType = (RemoteConnectionType)packetStream.ReadUInt16();
                                    uint luPacketId = packetStream.ReadUInt32();

                                    packetStream.SkipBytes(1);

                                    IUnserializable gamePacket = null;

                                    switch (remoteConnectionType)
                                    {
                                        case RemoteConnectionType.General:
                                            {
                                                GeneralPacketId generalPacketId = (GeneralPacketId)luPacketId;

                                                switch (generalPacketId)
                                                {
                                                    case GeneralPacketId.MSG_SERVER_VERSION_CONFIRM:
                                                        {
                                                            gamePacket = new GeneralVersionConfirmPacket();
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            Server.Instance.Logger.Log("Received an unknown LU packet. generalPacketId=" + generalPacketId);
                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        case RemoteConnectionType.Auth:
                                            {
                                                AuthPacketId authPacketId = (AuthPacketId)luPacketId;

                                                switch (authPacketId)
                                                {
                                                    case AuthPacketId.MSG_AUTH_LOGIN_REQUEST:
                                                        {
                                                            gamePacket = new AuthLoginRequestPacket();
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            Server.Instance.Logger.Log("Received an unknown LU packet. authPacketId=" + authPacketId);
                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        case RemoteConnectionType.World:
                                            {
                                                WorldPacketId worldPacketId = (WorldPacketId)luPacketId;

                                                switch (worldPacketId)
                                                {
                                                    case WorldPacketId.MSG_WORLD_CLIENT_VALIDATION:
                                                        {
                                                            gamePacket = new WorldValidationPacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_CHARACTER_LIST_REQUEST:
                                                        {
                                                            gamePacket = new WorldCharacterListRequestPacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_CHARACTER_CREATE_REQUEST:
                                                        {
                                                            gamePacket = new WorldCharacterCreateRequestPacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_CHARACTER_DELETE_REQUEST:
                                                        {
                                                            gamePacket = new WorldCharacterDeleteRequestPacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_CHARACTER_RENAME_REQUEST:
                                                        {
                                                            gamePacket = new WorldCharacterRenameRequestPacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_LOGIN_REQUEST:
                                                        {
                                                            gamePacket = new WorldLoginRequestPacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_LEVEL_LOAD_COMPLETE:
                                                        {
                                                            gamePacket = new WorldLevelLoadCompletePacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_POSITION_UPDATE:
                                                        {
                                                            gamePacket = new WorldPositionUpdatePacket();
                                                            break;
                                                        }
                                                    case WorldPacketId.MSG_WORLD_CLIENT_GAME_MSG:
                                                        {
                                                            long objectId = packetStream.ReadInt64();
                                                            ushort gameMessageId = packetStream.ReadUInt16();

                                                            switch (gameMessageId)
                                                            {
                                                                case (ushort)GameMessageId.PlayerLoaded:
                                                                    {
                                                                        gamePacket = new PlayerLoadedGameMessage(objectId);
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        Server.Instance.Logger.Log("Received an unknown LU packet. gameMessageId=" + gameMessageId);
                                                                        break;
                                                                    }
                                                            }
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            Server.Instance.Logger.Log("Received an unknown LU packet. worldPacketId=" + worldPacketId);
                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        default:
                                            {
                                                Server.Instance.Logger.Log("Received an unknown LU packet. remoteConnectionType=" + remoteConnectionType);
                                                break;
                                            }
                                    }

                                    if (gamePacket != null)
                                    {
                                        gamePacket.FromBitStream(packetStream);

                                        Server.Instance.Scheduler.RunTask(() => Server.Instance.EventManager.Publish(new GamePacketReceiveEvent(packet.SourceAddress, packet.SourcePort, packet.DestinationAddress, packet.DestinationPort, gamePacket)));
                                    }
                                }
                                catch (Exception exc)
                                {
                                    StackTrace stackTrace = new StackTrace(exc, true);
                                    StackFrame stackFrame = stackTrace.GetFrame(0);

                                    string fileName = stackFrame.GetFileName();
                                    int fileLine = stackFrame.GetFileLineNumber();
                                    int fileColumn = stackFrame.GetFileColumnNumber();

                                    Server.Instance.Logger.Log("Received an invalid game packet. address=" + packet.SourceAddress + " port=" + packet.SourcePort + " exception=" + exc.Message + " stackTrace=" + fileName + ":" + fileLine + ":" + fileColumn);
                                }

                                break;
                            }

                        default:
                            {
                                Server.Instance.Logger.Log("Received an unknown RakNet packet. rakNetPacketId=" + rakNetPacketId);
                                break;
                            }
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }
    }
}
