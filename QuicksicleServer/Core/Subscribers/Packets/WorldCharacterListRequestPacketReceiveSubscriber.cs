using System;
using Quicksicle.Sessions;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;
using Quicksicle.Core.Components;
using Quicksicle.Core.Helpers;

namespace Quicksicle.Core.Subscribers
{
    public class WorldCharacterListRequestPacketReceiveSubscriber
    {
        public WorldCharacterListRequestPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldCharacterListRequestPacketReceive);
        }

        public void OnWorldCharacterListRequestPacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldCharacterListRequestPacket)
            {
                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    Server.Instance.Logger.Log("A client requested the character list. address=" + e.SourceAddress + " port=" + e.SourcePort);

                    CharacterHelper.SaveCharacter(session);

                    Server.Instance.EventManager.Publish(new CharacterListRequestEvent(session));
                }
            }
        }
    }
}
