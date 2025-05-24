using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbConnectionHandler _connectionHandler;

        public ClienteRepository(IDbConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }

        public string InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<ClienteResponseInfra>(
                    "SELECT Cpf, Nome FROM Clientes WHERE Cpf = @Cpf", 
                    new { Cpf = cpf }
                );
            }
        }   ''
    }
}