using System;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Enums;

using Quicksicle.Core.Components;

namespace Quicksicle.Core.Helpers
{
    public static class CharacterHelper
    {
        public static void SaveCharacter(Session session)
        {
            if (session.ActiveCharacterInfo != null)
            {
                if (session.ActiveCharacterReplica != null)
                {
                    ControllablePhysicsComponent controllablePhysicsComponent = (ControllablePhysicsComponent)session.ActiveCharacterReplica.GetComponent(ReplicaComponentId.ControllablePhysics);

                    session.ActiveCharacterInfo.Position = controllablePhysicsComponent.Position;
                    session.ActiveCharacterInfo.Rotation = controllablePhysicsComponent.Rotation;

                    Server.Instance.GetReplicaManager(session.ActiveCharacterInfo.ZoneId).RemovePlayer(session);
                }

                CharacterInfo characterInfo = session.ActiveCharacterInfo;

                Server.Instance.Scheduler.RunTaskAsync(

                    () =>
                    {
                        MySqlHandle mySqlHandle = Server.Instance.DatabaseManager.GetMySqlHandle();

                        try
                        {
                            mySqlHandle.Open();
                            mySqlHandle.CharactersSetCharacterInfo(characterInfo);
                            mySqlHandle.Close();
                        }
                        catch (Exception exc)
                        {
                            Server.Instance.LogDatabaseError(exc);
                        }

                        mySqlHandle.Free();
                    }

                );
            }
        }
    }
}
