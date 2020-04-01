using System;
using System.Collections.Generic;
using System.Threading;
using Quicksicle.Database;

namespace Quicksicle.Sessions
{
    public class SessionManager
    {
        private List<Session> sessions;

        public SessionManager()
        {
            this.sessions = new List<Session>();
        }

        public Session GetSession(string address, ushort port)
        {
            Session session = null;

            Monitor.Enter(sessions);

            for (int i = 0; i < sessions.Count && session == null; i++)
            {
                Session current = sessions[i];

                if (current.Address == address && current.Port == port)
                {
                    session = current;
                }
            }

            Monitor.Exit(sessions);

            return session;
        }

        public Session GetSession(string username)
        {
            Session session = null;

            Monitor.Enter(sessions);

            for (int i = 0; i < sessions.Count && session == null; i++)
            {
                Session current = sessions[i];

                if (current.ActiveAccountInfo.Username == username)
                {
                    session = current;
                }
            }

            Monitor.Exit(sessions);

            return session;
        }

        public Session CreateSession(string address, ushort port, AccountInfo activeAccountInfo)
        {
            Session session = new Session(address, port, activeAccountInfo);

            Monitor.Enter(sessions);

            sessions.Add(session);

            Monitor.Exit(sessions);

            return session;
        }

        public void DeleteSession(string address, ushort port)
        {
            Monitor.Enter(sessions);

            Session session = null;

            for (int i = 0; i < sessions.Count && session == null; i++)
            {
                Session current = sessions[i];

                if (current.Address == address && current.Port == port)
                {
                    session = current;
                }
            }

            if (session != null)
            {
                sessions.Remove(session);
            }

            Monitor.Exit(sessions);
        }

        public void DeleteSession(Session session)
        {
            Monitor.Enter(sessions);

            sessions.Remove(session);

            Monitor.Exit(sessions);
        }
    }
}
