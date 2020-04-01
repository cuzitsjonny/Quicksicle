using System;
using System.Collections.Generic;
using Quicksicle.Other;
using MySql.Data.MySqlClient;
using Jitter.LinearMath;

namespace Quicksicle.Database
{
    public class MySqlHandle
    {
        private DatabaseManager databaseManager;
        private MySqlConnection connection;

        public MySqlHandle(DatabaseManager databaseManager, MySqlConnection connection)
        {
            this.databaseManager = databaseManager;
            this.connection = connection;
        }

        private MySqlCommand Prepare(string query)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText = query;
            command.Connection = connection;

            return command;
        }

        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public void Free()
        {
            databaseManager.FreeMySqlConnection(connection);

            connection = null;
        }

        public AccountInfo AccountsGetAccountInfo(string username)
        {
            AccountInfo accountInfo = null;

            string query = "SELECT id, password_salt, password_hash, password_iterations FROM accounts WHERE username = @username;";

            using (MySqlCommand command = Prepare(query))
            {
                command.Parameters.Add("@username", MySqlDbType.VarString).Value = username;

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            long accountId = reader.GetInt64("id");
                            string passwordSaltHex = reader.GetString("password_salt");
                            string passwordHashHex = reader.GetString("password_hash");
                            int passwordIterations = reader.GetInt32("password_iterations");

                            accountInfo = new AccountInfo(accountId, username, passwordSaltHex, passwordHashHex, passwordIterations);
                        }
                    }
                }
            }

            return accountInfo;
        }

        public List<CharacterInfo> CharactersGetCharacterInfos(long accountId)
        {
            List<CharacterInfo> characterInfos = new List<CharacterInfo>();

            string query = "SELECT id, name, pending_name, pending_name_rejected, head_color, head, chest_color, chest, legs, hair_style, hair_color, left_hand, right_hand, eyebrow_style, eyes_style, mouth_style, zone_id, clone_id, last_logout, position_x, position_y, position_z, rotation_x, rotation_y, rotation_z, rotation_w, gm_level, editor_level, current_health, max_health, current_armor, max_armor, current_imagination, max_imagination FROM characters WHERE account_id = @account_id ORDER BY last_logout DESC;";

            using (MySqlCommand command = Prepare(query))
            {
                command.Parameters.Add("@account_id", MySqlDbType.Int64).Value = accountId;

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            long characterId = reader.GetInt64("id");

                            CharacterInfo characterInfo = new CharacterInfo(characterId, accountId);

                            characterInfo.Name = reader.GetString("name");
                            characterInfo.PendingName = reader.GetString("pending_name");
                            characterInfo.PendingNameRejected = reader.GetBoolean("pending_name_rejected");
                            characterInfo.HeadColor = reader.GetUInt32("head_color");
                            characterInfo.Head = reader.GetUInt32("head");
                            characterInfo.ChestColor = reader.GetUInt32("chest_color");
                            characterInfo.Chest = reader.GetUInt32("chest");
                            characterInfo.Legs = reader.GetUInt32("legs");
                            characterInfo.HairStyle = reader.GetUInt32("hair_style");
                            characterInfo.HairColor = reader.GetUInt32("hair_color");
                            characterInfo.LeftHand = reader.GetUInt32("left_hand");
                            characterInfo.RightHand = reader.GetUInt32("right_hand");
                            characterInfo.EyebrowStyle = reader.GetUInt32("eyebrow_style");
                            characterInfo.EyesStyle = reader.GetUInt32("eyes_style");
                            characterInfo.MouthStyle = reader.GetUInt32("mouth_style");
                            characterInfo.ZoneId = reader.GetUInt16("zone_id");
                            characterInfo.CloneId = reader.GetUInt32("clone_id");
                            characterInfo.LastLogout = (ulong)reader.GetDateTime("last_logout").Subtract(Time.UnixEpoch).TotalSeconds;

                            float posX = reader.GetFloat("position_x");
                            float posY = reader.GetFloat("position_y");
                            float posZ = reader.GetFloat("position_z");

                            characterInfo.Position = new JVector(posX, posY, posZ);

                            float rotX = reader.GetFloat("rotation_x");
                            float rotY = reader.GetFloat("rotation_y");
                            float rotZ = reader.GetFloat("rotation_z");
                            float rotW = reader.GetFloat("rotation_w");

                            characterInfo.Rotation = new JQuaternion(rotX, rotY, rotZ, rotW);

                            characterInfo.GmLevel = reader.GetByte("gm_level");
                            characterInfo.EditorLevel = reader.GetByte("editor_level");
                            characterInfo.CurrentHealth = reader.GetByte("current_health");
                            characterInfo.MaxHealth = reader.GetByte("max_health");
                            characterInfo.CurrentArmor = reader.GetByte("current_armor");
                            characterInfo.MaxArmor = reader.GetByte("max_armor");
                            characterInfo.CurrentImagination = reader.GetByte("current_imagination");
                            characterInfo.MaxImagination = reader.GetByte("max_imagination");

                            characterInfos.Add(characterInfo);
                        }
                    }
                }
            }

            return characterInfos;
        }

        public bool CharactersCreateCharacter(CharacterInfo characterInfo)
        {
            bool success = false;

            string query = "INSERT INTO characters (id, account_id, name, head_color, head, chest_color, chest, legs, hair_style, hair_color, left_hand, right_hand, eyebrow_style, eyes_style, mouth_style, " +
                            "zone_id, clone_id, last_logout, position_x, position_y, position_z, rotation_x, rotation_y, rotation_z, rotation_w)" +
                            " SELECT @id, @account_id, @name, @head_color, @head, @chest_color, @chest, @legs, @hair_style, @hair_color, @left_hand, @right_hand, @eyebrow_style, @eyes_style, @mouth_style, " +
                            "@zone_id, @clone_id, @last_logout, @position_x, @position_y, @position_z, @rotation_x, @rotation_y, @rotation_z, @rotation_w FROM accounts" +
                            " WHERE id = @account_id AND IF((SELECT COUNT(*) FROM characters WHERE name = @name OR pending_name = @name) < 1, 1, 0) = 1;";

            using (MySqlCommand command = Prepare(query))
            {
                command.Parameters.Add("@id", MySqlDbType.Int64).Value = characterInfo.CharacterId;
                command.Parameters.Add("@account_id", MySqlDbType.Int64).Value = characterInfo.AccountId;
                command.Parameters.Add("@name", MySqlDbType.VarString).Value = characterInfo.Name;
                command.Parameters.Add("@head_color", MySqlDbType.UInt32).Value = characterInfo.HeadColor;
                command.Parameters.Add("@head", MySqlDbType.UInt32).Value = characterInfo.Head;
                command.Parameters.Add("@chest_color", MySqlDbType.UInt32).Value = characterInfo.ChestColor;
                command.Parameters.Add("@chest", MySqlDbType.UInt32).Value = characterInfo.Chest;
                command.Parameters.Add("@legs", MySqlDbType.UInt32).Value = characterInfo.Legs;
                command.Parameters.Add("@hair_style", MySqlDbType.UInt32).Value = characterInfo.HairStyle;
                command.Parameters.Add("@hair_color", MySqlDbType.UInt32).Value = characterInfo.HairColor;
                command.Parameters.Add("@left_hand", MySqlDbType.UInt32).Value = characterInfo.LeftHand;
                command.Parameters.Add("@right_hand", MySqlDbType.UInt32).Value = characterInfo.RightHand;
                command.Parameters.Add("@eyebrow_style", MySqlDbType.UInt32).Value = characterInfo.EyebrowStyle;
                command.Parameters.Add("@eyes_style", MySqlDbType.UInt32).Value = characterInfo.EyesStyle;
                command.Parameters.Add("@mouth_style", MySqlDbType.UInt32).Value = characterInfo.MouthStyle;
                command.Parameters.Add("@zone_id", MySqlDbType.UInt16).Value = characterInfo.ZoneId;
                command.Parameters.Add("@clone_id", MySqlDbType.UInt32).Value = characterInfo.CloneId;
                command.Parameters.Add("@last_logout", MySqlDbType.DateTime).Value = Time.UnixEpoch.AddSeconds(characterInfo.LastLogout);
                command.Parameters.Add("@position_x", MySqlDbType.Float).Value = characterInfo.Position.X;
                command.Parameters.Add("@position_y", MySqlDbType.Float).Value = characterInfo.Position.Y;
                command.Parameters.Add("@position_z", MySqlDbType.Float).Value = characterInfo.Position.Z;
                command.Parameters.Add("@rotation_x", MySqlDbType.Float).Value = characterInfo.Rotation.X;
                command.Parameters.Add("@rotation_y", MySqlDbType.Float).Value = characterInfo.Rotation.Y;
                command.Parameters.Add("@rotation_z", MySqlDbType.Float).Value = characterInfo.Rotation.Z;
                command.Parameters.Add("@rotation_w", MySqlDbType.Float).Value = characterInfo.Rotation.W;

                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    success = true;
                }
            }

            return success;
        }

        public void CharactersDeleteCharacter(long characterId)
        {
            string query = "DELETE FROM characters WHERE id = @id;";

            using (MySqlCommand command = Prepare(query))
            {
                command.Parameters.Add("@id", MySqlDbType.Int64).Value = characterId;

                command.ExecuteNonQuery();
            }
        }

        public void CharactersGetCharacterIds(List<long> idList)
        {
            string query = "SELECT id FROM characters;";

            using (MySqlCommand command = Prepare(query))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            long characterId = reader.GetInt64("id");

                            idList.Add(characterId);
                        }
                    }
                }
            }
        }

        public bool CharactersSetPendingName(long characterId, string pendingName)
        {
            bool success = false;

            string query = "UPDATE characters SET pending_name_rejected = 0, pending_name = @pending_name WHERE id = @id AND EXISTS(SELECT * FROM (SELECT COUNT(*) AS count FROM characters WHERE name = @pending_name OR pending_name = @pending_name) AS nested1 WHERE nested1.count < 1);";

            using (MySqlCommand command = Prepare(query))
            {
                command.Parameters.Add("@pending_name", MySqlDbType.VarString).Value = pendingName;
                command.Parameters.Add("@id", MySqlDbType.Int64).Value = characterId;

                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    success = true;
                }
            }

            return success;
        }

        public void CharactersUpdateLastLogout(long characterId)
        {
            string query = "UPDATE characters SET last_logout = @last_logout WHERE id = @id;";

            using (MySqlCommand command = Prepare(query))
            {
                command.Parameters.Add("@last_logout", MySqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@id", MySqlDbType.Int64).Value = characterId;

                command.ExecuteNonQuery();
            }
        }

        public void CharactersSetCharacterInfo(CharacterInfo characterInfo)
        {
            string query = "UPDATE characters SET zone_id = @zone_id, clone_id = @clone_id, last_logout = @last_logout, position_x = @position_x, position_y = @position_y, position_z = @position_z, rotation_x = @rotation_x, rotation_y = @rotation_y, rotation_z = @rotation_z, rotation_w = @rotation_w, gm_level = @gm_level, editor_level = @editor_level, current_health = @current_health, current_armor = @current_armor, current_imagination = @current_imagination, max_health = @max_health, max_armor = @max_armor, max_imagination = @max_imagination WHERE id = @id;";

            using (MySqlCommand command = Prepare(query))
            {
                command.Parameters.Add("@zone_id", MySqlDbType.UInt16).Value = characterInfo.ZoneId;
                command.Parameters.Add("@clone_id", MySqlDbType.UInt32).Value = characterInfo.CloneId;
                command.Parameters.Add("@last_logout", MySqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@position_x", MySqlDbType.Float).Value = characterInfo.Position.X;
                command.Parameters.Add("@position_y", MySqlDbType.Float).Value = characterInfo.Position.Y;
                command.Parameters.Add("@position_z", MySqlDbType.Float).Value = characterInfo.Position.Z;
                command.Parameters.Add("@rotation_x", MySqlDbType.Float).Value = characterInfo.Rotation.X;
                command.Parameters.Add("@rotation_y", MySqlDbType.Float).Value = characterInfo.Rotation.Y;
                command.Parameters.Add("@rotation_z", MySqlDbType.Float).Value = characterInfo.Rotation.Z;
                command.Parameters.Add("@rotation_w", MySqlDbType.Float).Value = characterInfo.Rotation.W;
                command.Parameters.Add("@gm_level", MySqlDbType.Byte).Value = characterInfo.GmLevel;
                command.Parameters.Add("@editor_level", MySqlDbType.Byte).Value = characterInfo.EditorLevel;
                command.Parameters.Add("@current_health", MySqlDbType.Byte).Value = characterInfo.CurrentHealth;
                command.Parameters.Add("@current_armor", MySqlDbType.Byte).Value = characterInfo.CurrentArmor;
                command.Parameters.Add("@current_imagination", MySqlDbType.Byte).Value = characterInfo.CurrentImagination;
                command.Parameters.Add("@max_health", MySqlDbType.Byte).Value = characterInfo.MaxHealth;
                command.Parameters.Add("@max_armor", MySqlDbType.Byte).Value = characterInfo.MaxArmor;
                command.Parameters.Add("@max_imagination", MySqlDbType.Byte).Value = characterInfo.MaxImagination;
                command.Parameters.Add("@id", MySqlDbType.Int64).Value = characterInfo.CharacterId;

                command.ExecuteNonQuery();
            }
        }
    }
}
