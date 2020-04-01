using System;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class ServerShutdownSubscriber
    {
        public ServerShutdownSubscriber()
        {
            Server.Instance.EventManager.Subscribe<ServerShutdownEvent>(OnServerShutdown);
        }

        public void OnServerShutdown(ServerShutdownEvent e)
        {

        }
    }
}
