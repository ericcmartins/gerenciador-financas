using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<ContaRepository> _logger;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public ContaRepository(ISqlServerConnectionHandler connectionHandler,
                                    NotificationPool notificationPool,
                                    ILogger<ContaRepository> logger)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        public async Task<List<ContaResponseInfra?>> GetContasPorUsuario(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<ContaResponseInfra>(SqlQueries.Contas.GetContas, new { idUsuario });
            var responseList = response.ToList();
            if (!responseList.Any())
            {
                _logger.LogWarning("Não foram encontradas contas para o usuário {IdUsuario}.", idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas contas para o usuário na base");
            }

            return responseList;
        }

        public async Task<bool> InsertConta(CadastrarContaRequestInfra contaRequest, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Contas.InsertConta, new
            {
                contaRequest.IdTipoConta,
                contaRequest.Instituicao,
                contaRequest.NumeroConta,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao cadastrar conta para o usuário {IdUsuario}.", idUsuario);
                _notificationPool.AddNotification(500, "Erro ao cadastrar conta na base");
                return false;
            }

            _logger.LogInformation("Conta para o usuário {IdUsuario} cadastrada com sucesso.", idUsuario);
            return true;
        }

        public async Task<bool> UpdateConta(AtualizarContaRequestInfra contaRequest, int idUsuario, int idConta)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Contas.UpdateConta, new
            {
                contaRequest.IdTipoConta,
                contaRequest.Instituicao,
                contaRequest.NumeroConta,
                IdUsuario = idUsuario,
                IdConta = idConta,
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao atualizar a conta {IdConta} para o usuário {IdUsuario}.", idConta, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao atualizar a conta na base");
                return false;
            }

            _logger.LogInformation("Conta {IdConta} do usuário {IdUsuario} atualizada com sucesso.", idConta, idUsuario);
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
                _logger.LogWarning("Tentativa de exclusão falhou. Conta {IdConta} do usuário {IdUsuario} não encontrada.", idConta, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao deletar conta da base");
                return false;
            }

            _logger.LogInformation("Conta {IdConta} do usuário {IdUsuario} excluída com sucesso.", idConta, idUsuario);
            return true;
        }
    }
}