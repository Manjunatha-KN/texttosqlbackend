using InnovationInc.TextToSql.WebApi.Interfaces;
using System.Data;
using System.Data.SQLite;

namespace azure_openai_quickstart.Services
{
    public class SQLiteService : IDbService
    {
        private readonly ILogger<SQLiteService> _logger;

        public SQLiteService(ILogger<SQLiteService> logger) 
        {
            _logger = logger;
        }

        public async Task<SQLiteConnection> ConnectToSQLiteAsync(CancellationToken cancellationToken)
        {
            var sqlConn = new SQLiteConnection("Data Source=:memory:;Version=3;");
            await sqlConn.OpenAsync(cancellationToken);
            return sqlConn;
        }

        // Execute the SQL query on the in-memory SQLite database and display the results
        public async Task<DataTable> ExecuteSqlQueryAsync(string sqlQuery, CancellationToken cancellationToken)
        {
            var sqlConnection = await ConnectToSQLiteAsync(cancellationToken);
            DataTable dataTable = new();

            try
            {
                using var command = new SQLiteCommand(sqlQuery, sqlConnection);
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                dataTable.Load(reader);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing SQL query: " + ex.Message);
            }

            return dataTable;
        }
    }
}
