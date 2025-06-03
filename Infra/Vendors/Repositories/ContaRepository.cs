using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public ContaRepository(ISqlServerConnectionHandler connectionHandler,
                               NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<ContaResponseInfra?>> GetContas(int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT IdConta, NumeroConta, Tipo, Instituicao, IdUsuario
                                 FROM Conta
                                 WHERE IdUsuario = @idUsuario";

            var response = await connection.QueryAsync<ContaResponseInfra>(instrucaoSql, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas contas para o usuário");

            return responseList;
        }

        public async Task<bool> InsertConta(ContaRequestInfra contaRequest, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO Conta (NumeroConta, Tipo, Instituicao, IdUsuario)
                                 VALUES (@NumeroConta, @Tipo, @Instituicao, @IdUsuario)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                contaRequest.NumeroConta, contaRequest.Tipo, contaRequest.Instituicao, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
               _notificationPool.AddNotification(500, "Erro ao cadastrar conta");
               return false;
            }

            return true;
        }

        public async Task<bool> UpdateConta(ContaRequestInfra contaRequest, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE Conta
                                SET Tipo = COALESCE(@Tipo, Tipo),
                                    Instituicao = COALESCE(@Instituicao, Instituicao)
                                WHERE NumeroConta = @NumeroConta
                                  AND IdUsuario = @IdUsuario
                                ";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                contaRequest.Tipo, contaRequest.Instituicao, contaRequest.NumeroConta, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
                return false;

            return true;
        }

        public async Task<bool> DeleteConta(string numeroConta, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM Conta WHERE NumeroConta = @NumeroConta AND IdUsuario = @IdUsuario";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                NumeroConta = numeroConta, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar as informações da conta");
                return false;
            }

            return true;
        }
    }
}
