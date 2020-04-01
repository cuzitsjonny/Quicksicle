using System;
using System.IO;
using Quicksicle.Sessions;
using Quicksicle.IO;
using Quicksicle.Packets;
using Quicksicle.Enums;
using Quicksicle.Net;
using Quicksicle.Objects;

using Quicksicle.Core.Events;
using Quicksicle.Core.Components;

namespace Quicksicle.Core.Subscribers
{
    public class WorldLevelLoadCompletePacketReceiveSubscriber
    {
        public WorldLevelLoadCompletePacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldLevelLoadCompletePacketReceive);
        }

        public void OnWorldLevelLoadCompletePacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldLevelLoadCompletePacket)
            {
                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    if (session.ActiveCharacterInfo != null)
                    {
                        WriteOnlyBinaryLdf ldfMap = new WriteOnlyBinaryLdf();

                        ldfMap.AddWideString("name", session.ActiveCharacterInfo.Name);
                        ldfMap.AddBoolean("editor_enabled", session.ActiveCharacterInfo.EditorLevel > 0);
                        ldfMap.AddInt32("editor_level", session.ActiveCharacterInfo.EditorLevel);
                        ldfMap.AddInt32("template", 1);
                        ldfMap.AddInt32("gmlevel", session.ActiveCharacterInfo.GmLevel);
                        ldfMap.AddUInt64("objid", (ulong)session.ActiveCharacterInfo.CharacterId);
                        ldfMap.AddFloat("position.x", session.ActiveCharacterInfo.Position.X);
                        ldfMap.AddFloat("position.y", session.ActiveCharacterInfo.Position.Y);
                        ldfMap.AddFloat("position.z", session.ActiveCharacterInfo.Position.Z);
                        ldfMap.AddFloat("rotation.x", session.ActiveCharacterInfo.Rotation.X);
                        ldfMap.AddFloat("rotation.y", session.ActiveCharacterInfo.Rotation.Y);
                        ldfMap.AddFloat("rotation.z", session.ActiveCharacterInfo.Rotation.Z);
                        ldfMap.AddFloat("rotation.w", session.ActiveCharacterInfo.Rotation.W);

                        ClientCreateCharacterPacket response = new ClientCreateCharacterPacket();

                        response.LdfMap = ldfMap;

                        Server.Instance.SendGamePacket(response, ClientPacketId.MSG_CLIENT_CREATE_CHARACTER, e.SourceAddress, e.SourcePort);

                        Replica character = new Replica(session.ActiveCharacterInfo.CharacterId, null, 1, 0);

                        character.Name = session.ActiveCharacterInfo.Name;

                        ControllablePhysicsComponent controllablePhysicsComponent = new ControllablePhysicsComponent();

                        controllablePhysicsComponent.Position = session.ActiveCharacterInfo.Position;
                        controllablePhysicsComponent.Rotation = session.ActiveCharacterInfo.Rotation;

                        character.AddComponent(controllablePhysicsComponent);

                        DestructibleComponent destructibleComponent = new DestructibleComponent();

                        character.AddComponent(destructibleComponent);

                        StatsComponent statsComponent = new StatsComponent();

                        statsComponent.IsSmashable = false;
                        statsComponent.FactionIds.Add(1);
                        statsComponent.CurrentHealth = session.ActiveCharacterInfo.CurrentHealth;
                        statsComponent.CurrentArmor = session.ActiveCharacterInfo.CurrentArmor;
                        statsComponent.CurrentImagination = session.ActiveCharacterInfo.CurrentImagination;
                        statsComponent.MaxHealth = session.ActiveCharacterInfo.MaxHealth;
                        statsComponent.MaxArmor = session.ActiveCharacterInfo.MaxArmor;
                        statsComponent.MaxImagination = session.ActiveCharacterInfo.MaxImagination;

                        character.AddComponent(statsComponent);

                        CharacterComponent characterComponent = new CharacterComponent();

                        characterComponent.CharacterInfo = session.ActiveCharacterInfo;
                        characterComponent.AccountInfo = session.ActiveAccountInfo;

                        character.AddComponent(characterComponent);

                        InventoryComponent inventoryComponent = new InventoryComponent();

                        character.AddComponent(inventoryComponent);

                        SkillComponent skillComponent = new SkillComponent();

                        character.AddComponent(skillComponent);

                        RenderComponent renderComponent = new RenderComponent();

                        character.AddComponent(renderComponent);

                        BbbComponent bbbComponent = new BbbComponent();

                        character.AddComponent(bbbComponent);

                        ReplicaManager thingManager = Server.Instance.GetReplicaManager(session.ActiveCharacterInfo.ZoneId);

                        thingManager.AddPlayer(session, character);

                        ServerDoneLoadingAllObjectGameMessage serverDoneLoadingAllObjects = new ServerDoneLoadingAllObjectGameMessage(session.ActiveCharacterInfo.CharacterId);

                        Server.Instance.SendGamePacket(serverDoneLoadingAllObjects, ClientPacketId.MSG_CLIENT_GAME_MSG, e.SourceAddress, e.SourcePort);
                    }
                    else
                    {
                        GeneralDisconnectNotifyPacket dcPacket = new GeneralDisconnectNotifyPacket();

                        dcPacket.DisconnectReason = DisconnectReason.CharacterCorruption;

                        Server.Instance.SendGamePacket(dcPacket, GeneralPacketId.MSG_SERVER_DISCONNECT_NOTIFY, e.SourceAddress, e.SourcePort);
                    }
                }
            }
        }
    }
}
