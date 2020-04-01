using System;
using Quicksicle.Events;
using Quicksicle.Sessions;

namespace Quicksicle.Core.Events
{
    public class CharacterListRequestEvent : BaseEvent
    {
        public CharacterListRequestEvent(Session session)
        {
            this.Session = session;
        }

        public Session Session
        {
            get;
        }
    }
}
