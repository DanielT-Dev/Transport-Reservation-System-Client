using Microsoft.Data.Sqlite;

namespace MPP_Client.Repository
{
    public static class DatabaseHelper
    {
        // Update this path to point to your Java app's .db file
        private const string DbPath = "Data Source=C:/Users/danie/Desktop/UBB/SEMESTRUL 4/PROGRAMMING ENVIRONMENTS/Repo/JavaGradleServer/mydatabase.db";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(DbPath);
        }
    }
}