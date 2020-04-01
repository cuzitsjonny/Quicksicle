using System;
using System.Data.SQLite;

namespace Quicksicle.Database
{
    public class SqliteHandle
    {
        private DatabaseManager databaseManager;
        private SQLiteConnection connection;

        public SqliteHandle(DatabaseManager databaseManager, SQLiteConnection connection)
        {
            this.databaseManager = databaseManager;
            this.connection = connection;
        }

        public void Free()
        {
            databaseManager.FreeSqliteConnection(connection);

            connection = null;
        }
    }
}
