using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace MPP_Client.Repository
{
    public static class DatabaseHelper
    {
        private static readonly ILogger _logger =
        LoggerManager.LoggerFactory.CreateLogger("DatabaseHelper");

        private static readonly string _connectionString;

        static DatabaseHelper()
        {
            _connectionString = "your-connection-string-here";
            _logger.LogInformation("DatabaseHelper initialized");
        }

        public static SqliteConnection GetConnection()
        {
            _logger.LogInformation("Opening SQLite connection");
            return new SqliteConnection(_connectionString);
        }
    }
}