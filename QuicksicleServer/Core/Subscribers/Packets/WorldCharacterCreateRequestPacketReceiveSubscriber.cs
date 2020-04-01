using System;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Other;
using Quicksicle.Packets;
using Quicksicle.Enums;
using Jitter.LinearMath;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class WorldCharacterCreateRequestPacketReceiveSubscriber
    {
        public WorldCharacterCreateRequestPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnWorldCharacterCreateRequestPacketReceive);
        }

        public void OnWorldCharacterCreateRequestPacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is WorldCharacterCreateRequestPacket)
            {
                WorldCharacterCreateRequestPacket request = (WorldCharacterCreateRequestPacket)e.Packet;

                Session session = Server.Instance.SessionManager.GetSession(e.SourceAddress, e.SourcePort);

                if (session != null)
                {
                    long characterId = Server.Instance.ObjectIdGenerator.GenerateGlobalId();

                    CharacterInfo characterInfo = new CharacterInfo(characterId, session.ActiveAccountInfo.AccountId);

                    string name;

                    if (Server.Instance.PredefinedNameCache != null)
                    {
                        name = Server.Instance.PredefinedNameCache.GetFirstName(request.PredefinedNameFirst);
                        name += Server.Instance.PredefinedNameCache.GetMiddleName(request.PredefinedNameMiddle);
                        name += Server.Instance.PredefinedNameCache.GetLastName(request.PredefinedNameLast);
                    }
                    else
                    {
                        name = characterId.ToString();
                    }

                    characterInfo.Name = name;
                    characterInfo.HeadColor = request.HeadColor;
                    characterInfo.Head = request.Head;
                    characterInfo.ChestColor = request.ChestColor;
                    characterInfo.Chest = request.Chest;
                    characterInfo.Legs = request.Legs;
                    characterInfo.HairStyle = request.HairStyle;
                    characterInfo.HairColor = request.HairColor;
                    characterInfo.LeftHand = request.LeftHand;
                    characterInfo.RightHand = request.RightHand;
                    characterInfo.EyebrowStyle = request.EyebrowStyle;
                    characterInfo.EyesStyle = request.EyesStyle;
                    characterInfo.MouthStyle = request.MouthStyle;
                    characterInfo.ZoneId = 0;
                    characterInfo.CloneId = 0;
                    characterInfo.LastLogout = (ulong)DateTime.Now.Subtract(Time.UnixEpoch).TotalSeconds;

                    if (Server.Instance.GetZone(1000) != null)
                    {
                        characterInfo.Position = Server.Instance.GetZone(1000).SpawnPosition;
                        characterInfo.Rotation = Server.Instance.GetZone(1000).SpawnRotation;
                    }
                    else
                    {
                        characterInfo.Position = JVector.Zero;
                        characterInfo.Rotation = new JQuaternion(0.0f, 0.0f, 0.0f, 0.0f);
                    }

                    Server.Instance.Scheduler.RunTaskAsync(

                        () =>
                        {
                            ClientCharacterCreateResponsePacket response = new ClientCharacterCreateResponsePacket();

                            response.CharacterCreationResult = CharacterCreateResult.DenyInvalidCustomName;

                            MySqlHandle mySqlHandle = Server.Instance.DatabaseManager.GetMySqlHandle();

                            try
                            {
                                mySqlHandle.Open();

                                bool success = mySqlHandle.CharactersCreateCharacter(characterInfo);

                                if (success)
                                {
                                    if (!String.IsNullOrEmpty(request.CustomName))
                                    {
                                        success = mySqlHandle.CharactersSetPendingName(characterId, request.CustomName);
                                    }

                                    if (success)
                                    {
                                        response.CharacterCreationResult = CharacterCreateResult.Allow;
                                    }
                                    else
                                    {
                                        response.CharacterCreationResult = CharacterCreateResult.DenyCustomNameTaken;

                                        mySqlHandle.CharactersDeleteCharacter(characterId);
                                    }
                                }
                                else
                                {
                                    response.CharacterCreationResult = CharacterCreateResult.DenyPredefinedNameTaken;
                                }

                                mySqlHandle.Close();
                            }
                            catch (Exception exc)
                            {
                                Server.Instance.LogDatabaseError(exc);
                            }

                            mySqlHandle.Free();

                            Server.Instance.SendGamePacket(response, ClientPacketId.MSG_CLIENT_CHARACTER_CREATE_RESPONSE, e.SourceAddress, e.SourcePort);

                            Server.Instance.Logger.Log("A client requested to create a character. address=" + e.SourceAddress + " port=" + e.SourcePort + " predefinedName=" + name + " customName=" + request.CustomName + " result=" + response.CharacterCreationResult);

                            if (response.CharacterCreationResult == CharacterCreateResult.Allow)
                            {
                                Server.Instance.Scheduler.RunTask(() => Server.Instance.EventManager.Publish(new CharacterListRequestEvent(session)));
                            }
                            else
                            {
                                Server.Instance.ObjectIdGenerator.FreeObjectId(characterId);
                            }
                        }

                    );
                }
            }
        }
    }
}
