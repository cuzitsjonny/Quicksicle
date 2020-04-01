using System;
using System.Collections.Generic;
using Quicksicle.Database;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class CharacterListRequestSubscriber
    {
        public CharacterListRequestSubscriber()
        {
            Server.Instance.EventManager.Subscribe<CharacterListRequestEvent>(OnCharacterListRequest);
        }

        public void OnCharacterListRequest(CharacterListRequestEvent e)
        {
            Server.Instance.Scheduler.RunTaskAsync(

                    () =>
                    {
                        ClientCharacterListResponsePacket response = new ClientCharacterListResponsePacket();

                        MySqlHandle mySqlHandle = Server.Instance.DatabaseManager.GetMySqlHandle();

                        try
                        {
                            mySqlHandle.Open();

                            List<CharacterInfo> characterInfos = mySqlHandle.CharactersGetCharacterInfos(e.Session.ActiveAccountInfo.AccountId);

                            foreach (CharacterInfo characterInfo in characterInfos)
                            {
                                ClientCharacterListResponsePacket.CharacterData characterData = new ClientCharacterListResponsePacket.CharacterData();

                                characterData.CharacterInfo = characterInfo;

                                response.CharacterDataList.Add(characterData);
                            }

                            mySqlHandle.Close();
                        }
                        catch (Exception exc)
                        {
                            Server.Instance.LogDatabaseError(exc);
                        }

                        mySqlHandle.Free();

                        Server.Instance.SendGamePacket(response, ClientPacketId.MSG_CLIENT_CHARACTER_LIST_RESPONSE, e.Session.Address, e.Session.Port);
                    }

            );
        }
    }
}
