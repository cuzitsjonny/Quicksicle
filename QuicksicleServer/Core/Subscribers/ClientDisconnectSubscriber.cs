using System;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Enums;

using Quicksicle.Core.Events;
using Quicksicle.Core.Components;
using Quicksicle.Core.Helpers;

namespace Quicksicle.Core.Subscribers
{
    public class ClientDisconnectSubscriber
    {
        public ClientDisconnectSubscriber()
        {
            Server.Instance.EventManager.Subscribe<ClientDisconnectEvent>(OnClientDisconnect);
        }

        public void OnClientDisconnect(ClientDisconnectEvent e)
        {
            string instance = "AUTH";

            if (e.DestinationPort == Server.Instance.WorldPort)
            {
                instance = "WORLD";
            }

            Server.Instance.Logger.Log("A client disconnected from " + instance + ". address=" + e.SourceAddress + " port=" + e.SourcePort);

            if (e.DestinationPort == Server.Instance.WorldPort)
            {
                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    CharacterHelper.SaveCharacter(session);

                    Server.Instance.SessionManager.DeleteSession(session);
                }
            }
        }
    }
}
