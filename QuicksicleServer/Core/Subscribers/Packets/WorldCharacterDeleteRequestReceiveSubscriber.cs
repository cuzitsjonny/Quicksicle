using System;
using System.Collections.Generic;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class WorldCharacterDeleteRequestPacketReceiveSubscriber
    {
        public WorldCharacterDeleteRequestPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldCharacterDeleteRequestReceive);
        }

        public void OnWorldCharacterDeleteRequestReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldCharacterDeleteRequestPacket)
            {
                WorldCharacterDeleteRequestPacket request = (WorldCharacterDeleteRequestPacket)e.Packet;

                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    Server.Instance.Scheduler.RunTaskAsync(

                        () =>
                        {
                            long characterId = request.CharacterId;
                            string name = String.Empty;
                            bool isOwnedByRequester = false;

                            MySqlHandle mySqlHandle = Server.Instance.DatabaseManager.GetMySqlHandle();

                            try
                            {
                                mySqlHandle.Open();

                                List<CharacterInfo> characterInfos = mySqlHandle.CharactersGetCharacterInfos(session.ActiveAccountInfo.AccountId);

                                for (int i = 0; i < characterInfos.Count && !isOwnedByRequester; i++)
                                {
                                    if (characterInfos[i].CharacterId == characterId)
                                    {
                                        isOwnedByRequester = true;
                                        name = characterInfos[i].Name;

                                        mySqlHandle.CharactersDeleteCharacter(characterId);
                                    }
                                }

                                mySqlHandle.Close();
                            }
                            catch (Exception exc)
                            {
                                Server.Instance.LogDatabaseError(exc);
                            }

                            mySqlHandle.Free();

                            Server.Instance.Logger.Log("A client requested to delete a character. address=" + e.SourceAddress + " port=" + e.SourcePort + " name=" + name + " isOwnedByRequester=" + isOwnedByRequester);

                            if (!isOwnedByRequester)
                            {
                                GeneralDisconnectNotifyPacket dcPacket = new GeneralDisconnectNotifyPacket();

                                dcPacket.DisconnectReason = DisconnectReason.CharacterNotFound;

                                Server.Instance.SendGamePacket(dcPacket, GeneralPacketId.MSG_SERVER_DISCONNECT_NOTIFY, e.SourceAddress, e.SourcePort);
                            }
                        }

                    );
                }
            }
        }
    }
}
