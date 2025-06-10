using System.Data;
using Microsoft.Data.SqlClient;


namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class SqlServerConnectionHandler : ISqlServerConnectionHandler
    {
        private readonly string _connectionString;

        public SqlServerConnectionHandler(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}