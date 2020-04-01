using System;
using System.Collections.Generic;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class WorldLoginRequestPacketReceiveSubscriber
    {
        public WorldLoginRequestPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldLoginRequestPacketReceive);
        }

        public void OnWorldLoginRequestPacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldLoginRequestPacket)
            {
                WorldLoginRequestPacket request = (WorldLoginRequestPacket)e.Packet;

                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    Server.Instance.Scheduler.RunTaskAsync(

                        () =>
                        {
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

                                mySqlHandle.Close();
                            }
                            catch (Exception exc)
                            {
                                Server.Instance.LogDatabaseError(exc);
                            }

                            mySqlHandle.Free();

                            if (characterInfo != null)
                            {
                                if (session.ActiveCharacterInfo == null || session.ActiveCharacterInfo.CharacterId == characterInfo.CharacterId)
                                {
                                    if (characterInfo.ZoneId == 0)
                                    {
                                        characterInfo.ZoneId = 1000;
                                    }

                                    session.ActiveCharacterInfo = characterInfo;
                                }

                                ClientLoadStaticZonePacket response = new ClientLoadStaticZonePacket();

                                response.ZoneId = characterInfo.ZoneId;
                                response.CloneId = characterInfo.CloneId;
                                response.ZoneChecksum = Server.Instance.ZoneChecksumCache.GetZoneChecksum(characterInfo.ZoneId);
                                response.EditorEnabled = (characterInfo.EditorLevel) > 0 ? (byte)1 : (byte)0;
                                response.EditorLevel = characterInfo.EditorLevel;
                                response.Position = characterInfo.Position;

                                Server.Instance.SendGamePacket(response, ClientPacketId.MSG_CLIENT_LOAD_STATIC_ZONE, e.SourceAddress, e.SourcePort);
                            }
                            else
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
