using System;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using Quicksicle.Sessions;
using Quicksicle.Database;
using Quicksicle.Packets;
using Quicksicle.Enums;

using Quicksicle.Core.Events;

namespace Quicksicle.Core.Subscribers
{
    public class AuthLoginRequestPacketReceiveSubscriber
    {
        public AuthLoginRequestPacketReceiveSubscriber()
        {
            Server.Instance.EventManager.Subscribe<GamePacketReceiveEvent>(OnAuthLoginRequestPacketReceive);
        }

        public void OnAuthLoginRequestPacketReceive(GamePacketReceiveEvent e)
        {
            if (e.Packet is AuthLoginRequestPacket)
            {
                AuthLoginRequestPacket request = (AuthLoginRequestPacket)e.Packet;

                string username = request.Username;
                string password = request.Password;
                string maintenanceText = null;

                if (File.Exists("./maintenance.txt"))
                {
                    string[] lines = File.ReadAllLines("./maintenance.txt");

                    maintenanceText = String.Empty;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i > 0)
                        {
                            maintenanceText += "\n";
                        }

                        maintenanceText += lines[i];
                    }
                }

                Server.Instance.Scheduler.RunTaskAsync(

                    () =>
                    {
                        ClientLoginResponsePacket response = new ClientLoginResponsePacket();
                        AccountInfo accountInfo = null;

                        MySqlHandle mySqlHandle = Server.Instance.DatabaseManager.GetMySqlHandle();

                        try
                        {
                            mySqlHandle.Open();

                            accountInfo = mySqlHandle.AccountsGetAccountInfo(username);

                            if (accountInfo != null)
                            {
                                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, accountInfo.PasswordSalt, accountInfo.PasswordIterations, HashAlgorithmName.SHA256);

                                byte[] passwordHash = pbkdf2.GetBytes(32);

                                if (passwordHash.SequenceEqual(accountInfo.PasswordHash))
                                {
                                    response.LoginResult = LoginResult.Allow;
                                }
                                else
                                {
                                    response.LoginResult = LoginResult.DenyInvalidCredentials;
                                }
                            }
                            else
                            {
                                response.LoginResult = LoginResult.DenyInvalidCredentials;
                            }

                            mySqlHandle.Close();
                        }
                        catch (Exception exc)
                        {
                            Server.Instance.LogDatabaseError(exc);
                        }

                        mySqlHandle.Free();

                        if (maintenanceText != null)
                        {
                            response.LoginResult = LoginResult.DenyCustomError;

                            response.CustomErrorMessage = maintenanceText;
                        }

                        if (response.LoginResult == LoginResult.Allow)
                        {
                            Session session = Server.Instance.SessionManager.CreateSession(e.SourceAddress, e.SourcePort, accountInfo);

                            response.SessionSecret = session.Secret;
                        }
                        else
                        {
                            response.SessionSecret = "AUTHENTICATION FAILED";
                        }

                        response.CharacterInstanceIp = e.DestinationAddress;
                        response.ChatInstanceIp = e.DestinationAddress;
                        response.CharacterInstancePort = Server.Instance.WorldPort;
                        response.ChatInstancePort = Server.Instance.WorldPort;

                        Server.Instance.SendGamePacket(response, ClientPacketId.MSG_CLIENT_LOGIN_RESPONSE, e.SourceAddress, e.SourcePort);

                        Server.Instance.Logger.Log("A client requested to authenticate. address=" + e.SourceAddress + " port=" + e.SourcePort + " username=" + username + " result=" + response.LoginResult);
                    }

                );
            }
        }
    }
}
