using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
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
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<ContaResponseInfra>(SqlQueries.Contas.GetContas, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas contas para o usuário");

            return responseList;
        }

        public async Task<bool> InsertConta(ContaRequestInfra contaRequest, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Contas.InsertConta, new
            {
                contaRequest.NumeroConta,
                contaRequest.Tipo,
                contaRequest.Instituicao,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar conta");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateConta(ContaRequestInfra contaRequest, int idUsuario, int idConta)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Contas.UpdateConta, new
            {
                contaRequest.Tipo,
                contaRequest.Instituicao,
                contaRequest.NumeroConta,
                IdUsuario = idUsuario,
                IdConta = idConta,
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar a conta");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteConta(int idConta, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Contas.DeleteConta, new
            {
                IdConta = idConta,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas == 0)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar conta");
                return false;
            }

            return true;
        }
    }
}
