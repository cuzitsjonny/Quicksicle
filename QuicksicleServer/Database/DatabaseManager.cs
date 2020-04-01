using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace Quicksicle.Database
{
    public class DatabaseManager
    {
        private string mySqlHost;
        private int mySqlPort;
        private string mySqlUser;
        private string mySqlPassword;
        private string mySqlDatabase;

        private string cdClientPath;
        private int sqlitePoolSize;

        private Dictionary<SQLiteConnection, bool> sqliteConnections;
        private List<MySqlConnection> mySqlConnections;

        public DatabaseManager(string mySqlHost, int mySqlPort, string mySqlUser, string mySqlPassword, string mySqlDatabase, string resourcesDirectoryPath, int sqlitePoolSize)
        {
            this.mySqlHost = mySqlHost;
            this.mySqlPort = mySqlPort;
            this.mySqlUser = mySqlUser;
            this.mySqlPassword = mySqlPassword;
            this.mySqlDatabase = mySqlDatabase;
            this.sqliteConnections = new Dictionary<SQLiteConnection, bool>();
            this.mySqlConnections = new List<MySqlConnection>();

            this.cdClientPath = Path.Combine(resourcesDirectoryPath, "CDClient.sqlite");
            this.sqlitePoolSize = sqlitePoolSize;
        }

        public MySqlHandle GetMySqlHandle()
        {
            MySqlHandle handle = null;

            Monitor.Enter(mySqlConnections);

            MySqlConnection connection = new MySqlConnection("Server = " + mySqlHost + "; Uid=" + mySqlUser + "; Pwd=" + mySqlPassword + "; Database=" + mySqlDatabase + "; Port=" + mySqlPort + ";");

            handle = new MySqlHandle(this, connection);

            Monitor.Exit(mySqlConnections);

            return handle;
        }

        public SqliteHandle GetSqliteHandle()
        {
            SqliteHandle handle = null;

            do
            {
                Monitor.Enter(sqliteConnections);

                foreach (SQLiteConnection connection in sqliteConnections.Keys)
                {
                    if (handle == null)
                    {
                        bool isFree = sqliteConnections[connection];

                        if (isFree)
                        {
                            sqliteConnections[connection] = !isFree;

                            handle = new SqliteHandle(this, connection);
                        }
                    }
                }

                Monitor.Exit(sqliteConnections);

                if (handle == null)
                {
                    Thread.Sleep(1);
                }
            } while (handle == null);

            return handle;
        }

        public void FreeMySqlConnection(MySqlConnection connection)
        {
            Monitor.Enter(mySqlConnections);

            mySqlConnections.Remove(connection);

            Monitor.Exit(mySqlConnections);
        }

        public void FreeSqliteConnection(SQLiteConnection connection)
        {
            Monitor.Enter(sqliteConnections);

            sqliteConnections[connection] = true; // isFree = true

            Monitor.Exit(sqliteConnections);
        }

        public void Start()
        {
            if (File.Exists(cdClientPath))
            {
                for (int i = 0; i < sqlitePoolSize; i++)
                {
                    SQLiteConnection connection = new SQLiteConnection("Data Source = " + cdClientPath + "; Version = 3;");

                    connection.Open();

                    sqliteConnections.Add(connection, true);
                }
            }
            else
            {
                throw new FileNotFoundException("Unable to connect to CDClient.sqlite.");
            }
        }

        public void ShutDown()
        {
            int connectionsLeft;

            do
            {
                Monitor.Enter(sqliteConnections);

                connectionsLeft = sqliteConnections.Count;

                SQLiteConnection removable = null;

                foreach (SQLiteConnection connection in sqliteConnections.Keys)
                {
                    bool isFree = sqliteConnections[connection];

                    if (isFree)
                    {
                        removable = connection;
                    }
                }

                if (removable != null)
                {
                    sqliteConnections.Remove(removable);

                    connectionsLeft--;
                }

                Monitor.Exit(sqliteConnections);

                if (connectionsLeft > 0)
                {
                    Thread.Sleep(1);
                }
            } while (connectionsLeft > 0);

            do
            {
                Monitor.Enter(mySqlConnections);

                connectionsLeft = mySqlConnections.Count;

                Monitor.Exit(mySqlConnections);

                if (connectionsLeft > 0)
                {
                    Thread.Sleep(1);
                }
            } while (connectionsLeft > 0);
        }
    }
}
