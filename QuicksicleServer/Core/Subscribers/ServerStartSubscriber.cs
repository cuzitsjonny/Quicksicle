using System;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class ServerStartSubscriber
    {
        public ServerStartSubscriber()
        {
            Server.Instance.EventManager.Subscribe<ServerStartEvent>(OnServerStart);
        }

        public void OnServerStart(ServerStartEvent e)
        {
            new GeneralVersionConfirmPacketReceiveSubscriber();
            new AuthLoginRequestPacketReceiveSubscriber();
            new WorldValidationPacketReceiveSubscriber();
            new WorldCharacterListRequestPacketReceiveSubscriber();
            new WorldCharacterCreateRequestPacketReceiveSubscriber();
            new WorldCharacterDeleteRequestPacketReceiveSubscriber();
            new WorldCharacterRenameRequestPacketReceiveSubscriber();
            new WorldLoginRequestPacketReceiveSubscriber();
            new WorldLevelLoadCompletePacketReceiveSubscriber();
            new WorldPositionUpdatePacketReceiveSubscriber();

            new PlayerLoadedGameMessageReceiveSubscriber();

            new CharacterListRequestSubscriber();
        }
    }
}
