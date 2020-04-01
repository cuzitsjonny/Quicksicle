using System;
using System.Collections.Generic;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class WorldCharacterRenameRequestPacketReceiveSubscriber
    {
        public WorldCharacterRenameRequestPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldCharacterRenameRequestPacketReceive);
        }

        public void OnWorldCharacterRenameRequestPacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldCharacterRenameRequestPacket)
            {
                WorldCharacterRenameRequestPacket request = (WorldCharacterRenameRequestPacket)e.Packet;

                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    Server.Instance.Scheduler.RunTaskAsync(

                        () =>
                        {
                            ClientCharacterRenameResponsePacket response = new ClientCharacterRenameResponsePacket();

                            response.CharacterRenameResult = CharacterRenameResult.DenyInvalidCustomName;

                            string oldName = String.Empty;
                            string newName = request.NewName;
                            long characterId = request.CharacterId;
                            CharacterInfo characterInfo = null;

                            MySqlHandle mySqlHandle = Server.Instance.DatabaseManager.GetMySqlHandle();

                            try
                            {
                                mySqlHandle.Open();

                                List<CharacterInfo> characterInfos = mySqlHandle.CharactersGetCharacterInfos(session.ActiveAccountInfo.AccountId);

                                for (int i = 0; i < characterInfos.Count && characterInfo == null; i++)
                                {
                                    if (characterInfos[i].CharacterId == characterId)
                                    {
                                        characterInfo = characterInfos[i];
                                    }
                                }

                                if (characterInfo != null)
                                {
                                    oldName = characterInfo.Name;

                                    bool success = mySqlHandle.CharactersSetPendingName(characterId, newName);

                                    if (success)
                                    {
                                        response.CharacterRenameResult = CharacterRenameResult.Allow;
                                    }
                                    else
                                    {
                                        response.CharacterRenameResult = CharacterRenameResult.DenyCustomNameTaken;

                                        mySqlHandle.CharactersUpdateLastLogout(characterId);
                                    }
                                }

                                mySqlHandle.Close();
                            }
                            catch (Exception exc)
                            {
                                Server.Instance.LogDatabaseError(exc);
                            }

                            mySqlHandle.Free();

                            Server.Instance.SendGamePacket(response, ClientPacketId.MSG_CLIENT_CHARACTER_RENAME_RESPONSE, e.SourceAddress, e.SourcePort);

                            Server.Instance.Scheduler.RunTask(() => Server.Instance.EventManager.Publish(new CharacterListRequestEvent(session)));

                            Server.Instance.Logger.Log("A client requested to rename a character. address=" + e.SourceAddress + " port=" + e.SourcePort + " oldName=" + oldName + " newName=" + newName + " isOwnedByRequester=" + (characterInfo != null));

                            if (characterInfo != null)
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
