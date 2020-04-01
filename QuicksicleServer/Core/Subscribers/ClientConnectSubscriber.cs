using System;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class ClientConnectSubscriber
    {
        public ClientConnectSubscriber()
        {
            Server.Instance.EventManager.Subscribe<ClientConnectEvent>(OnClientConnect);
        }

        public void OnClientConnect(ClientConnectEvent e)
        {
            string instance = "AUTH";

            if (e.DestinationPort == Server.Instance.WorldPort)
            {
                instance = "WORLD";
            }

            Server.Instance.Logger.Log("A client connected to " + instance + ". address=" + e.SourceAddress + " port=" + e.SourcePort);
        }
    }
}
