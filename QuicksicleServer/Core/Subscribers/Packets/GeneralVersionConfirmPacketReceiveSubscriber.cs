using System;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class GeneralVersionConfirmPacketReceiveSubscriber
    {
        public GeneralVersionConfirmPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnGeneralVersionConfirmPacketReceive);
        }

        public void OnGeneralVersionConfirmPacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is GeneralVersionConfirmPacket)
            {
                GeneralVersionConfirmPacket request = (GeneralVersionConfirmPacket)e.Packet;

                if (e.DestinationPort == Server.Instance.AuthPort)
                {
                    request.RemoteConnectionType = 1;
                }
                else
                {
                    request.RemoteConnectionType = 4;
                }

                Server.Instance.SendGamePacket(request, GeneralPacketId.MSG_SERVER_VERSION_CONFIRM, e.SourceAddress, e.SourcePort);
            }
        }
    }
}
