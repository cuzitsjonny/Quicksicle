using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Packets;
using Quicksicle.Sessions;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class PlayerLoadedGameMessageReceiveSubscriber
    {
        public PlayerLoadedGameMessageReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnPlayerLoadedGameMessageReceive);
        }

        public void OnPlayerLoadedGameMessageReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is PlayerLoadedGameMessage)
            {
                PlayerLoadedGameMessage request = (PlayerLoadedGameMessage)e.Packet;

                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    if (session.ActiveCharacterInfo != null)
                    {
                        PlayerReadyGameMessage playerReady = new PlayerReadyGameMessage(session.ActiveCharacterInfo.CharacterId);

                        Server.Instance.SendGamePacket(playerReady, ClientPacketId.MSG_CLIENT_GAME_MSG, e.SourceAddress, e.SourcePort);
                    }
                }
            }
        }
    }
}
