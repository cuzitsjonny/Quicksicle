using System;
using Quicksicle.Sessions;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class WorldValidationPacketReceiveSubscriber
    {
        public WorldValidationPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldValidationPacketReceive);
        }

        public void OnWorldValidationPacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldValidationPacket)
            {
                WorldValidationPacket request = (WorldValidationPacket)e.Packet;

                string username = request.Username;
                string sessionSecret = request.SessionKey;
                DisconnectReason dcReason = DisconnectReason.NoReason;

                Session session = Server.Instance.SessionManager.GetSession(username);

                if (session != null)
                {
                    if (session.Secret == sessionSecret)
                    {
                        session.Address = e.SourceAddress;
                        session.Port = e.SourcePort;
                    }
                    else
                    {
                        dcReason = DisconnectReason.InvalidSessionKey;
                    }
                }
                else
                {
                    dcReason = DisconnectReason.UnauthenticatedAccount;
                }

                if (dcReason != DisconnectReason.NoReason)
                {
                    GeneralDisconnectNotifyPacket dcPacket = new GeneralDisconnectNotifyPacket();

                    dcPacket.DisconnectReason = dcReason;

                    Server.Instance.SendGamePacket(dcPacket, GeneralPacketId.MSG_SERVER_DISCONNECT_NOTIFY, e.SourceAddress, e.SourcePort);
                }

                Server.Instance.Logger.Log("A client requested to validate their session. address=" + e.SourceAddress + " port=" + e.SourcePort + " username=" + username + " dcReason=" + dcReason);
            }
        }
    }
}
