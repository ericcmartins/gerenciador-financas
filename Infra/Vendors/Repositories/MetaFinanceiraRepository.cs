using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class MetaFinanceiraRepository : IMetaFinanceiraRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<MetaFinanceiraRepository> _logger;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public MetaFinanceiraRepository(ISqlServerConnectionHandler connectionHandler,
                                        NotificationPool notificationPool,
                                        ILogger<MetaFinanceiraRepository> logger)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        public async Task<List<MetaFinanceiraResponseInfra?>> GetMetasFinanceiras(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<MetaFinanceiraResponseInfra>(SqlQueries.MetasFinanceiras.GetMetasFinanceiras, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
            {
                _logger.LogWarning("Não foram encontradas metas financeiras para o usuário {IdUsuario}.", idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas metas financeiras para o usuário na base");
            }

            return responseList;
        }

        public async Task<bool> InsertMetaFinanceira(CadastrarMetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetasFinanceiras.InsertMetaFinanceira, new
            {
                metaFinanceiraRequest.Nome,
                metaFinanceiraRequest.Descricao,
                metaFinanceiraRequest.ValorAlvo,
                metaFinanceiraRequest.ValorAtual,
                metaFinanceiraRequest.DataInicio,
                metaFinanceiraRequest.DataLimite,
                IdUsuario = idUsuario,
                Concluida = false
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao cadastrar meta financeira '{NomeMeta}' para o usuário {IdUsuario}.", metaFinanceiraRequest.Nome, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao cadastrar meta financeira na base");
                return false;
            }

            _logger.LogInformation("Meta financeira '{NomeMeta}' para o usuário {IdUsuario} cadastrada com sucesso.", metaFinanceiraRequest.Nome, idUsuario);
            return true;
        }

        public async Task<bool> UpdateMetaFinanceira(AtualizarMetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario, int idMetaFinanceira)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetasFinanceiras.UpdateMetaFinanceira, new
            {
                metaFinanceiraRequest.Descricao,
                metaFinanceiraRequest.ValorAlvo,
                metaFinanceiraRequest.ValorAtual,
                metaFinanceiraRequest.DataInicio,
                metaFinanceiraRequest.DataLimite,
                IdUsuario = idUsuario,
                IdMetaFinanceira = idMetaFinanceira
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao atualizar meta financeira {IdMetaFinanceira} para o usuário {IdUsuario}.", idMetaFinanceira, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao atualizar meta financeira na base");
                return false;
            }

            _logger.LogInformation("Meta financeira {IdMetaFinanceira} do usuário {IdUsuario} atualizada com sucesso.", idMetaFinanceira, idUsuario);
            return true;
        }

        public async Task<bool> DeleteMetaFinanceira(int idMetaFinanceira, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetasFinanceiras.DeleteMetaFinanceira, new
            {
                IdUsuario = idUsuario,
                IdMetaFinanceira = idMetaFinanceira
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogWarning("Tentativa de exclusão falhou. Meta financeira {IdMetaFinanceira} do usuário {IdUsuario} não encontrada ou erro ao deletar.", idMetaFinanceira, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao deletar meta financeira");
                return false;
            }

            _logger.LogInformation("Meta financeira {IdMetaFinanceira} do usuário {IdUsuario} excluída com sucesso.", idMetaFinanceira, idUsuario);
            return true;
        }
    }
}