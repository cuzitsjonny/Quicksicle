using System;
using Quicksicle.Packets;
using Quicksicle.Sessions;
using Quicksicle.Objects;
using Quicksicle.Enums;

using Quicksicle.Core.Events;
using Quicksicle.Core.Components;

namespace Quicksicle.Core.Subscribers
{
    public class WorldPositionUpdatePacketReceiveSubscriber
    {
        public WorldPositionUpdatePacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldPositionUpdatePacketReceive);
        }

        public void OnWorldPositionUpdatePacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldPositionUpdatePacket)
            {
                WorldPositionUpdatePacket request = (WorldPositionUpdatePacket)e.Packet;

                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    if (session.ActiveCharacterInfo != null)
                    {
                        ControllablePhysicsComponent controllablePhysicsComponent = (ControllablePhysicsComponent)session.ActiveCharacterReplica.GetComponent(ReplicaComponentId.ControllablePhysics);

                        controllablePhysicsComponent.Position = request.Position;
                        controllablePhysicsComponent.Rotation = request.Rotation;
                        controllablePhysicsComponent.IsSupported = request.IsSupported;
                        controllablePhysicsComponent.IsOnRail = request.IsOnRail;
                        controllablePhysicsComponent.LinearVelocity = request.LinearVelocity;
                        controllablePhysicsComponent.AngularVelocity = request.AngularVelocity;
                        controllablePhysicsComponent.LocalSpaceObjectId = request.LocalSpaceObjectId;
                        controllablePhysicsComponent.LocalPosition = request.LocalPosition;
                        controllablePhysicsComponent.LocalLinearVelocity = request.LocalLinearVelocity;

                        Server.Instance.GetReplicaManager(session.ActiveCharacterInfo.ZoneId).Update(session.ActiveCharacterReplica);
                    }
                }
            }
        }
    }
}
