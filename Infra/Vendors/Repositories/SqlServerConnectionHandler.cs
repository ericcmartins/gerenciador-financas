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

        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}