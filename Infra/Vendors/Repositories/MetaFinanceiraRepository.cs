using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class MetaFinanceiraRepository : IMetaFinanceiraRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public MetaFinanceiraRepository(ISqlServerConnectionHandler connectionHandler,
                                        NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<MetaFinanceiraResponseInfra?>> GetMetasFinanceiras(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var response = await connection.QueryAsync<MetaFinanceiraResponseInfra>(SqlQueries.MetaFinanceira.GetMetasFinanceiras, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas metas financeiras para o usuário");

            return responseList;
        }

        public async Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetaFinanceira.InsertMetaFinanceira, new
            {
                metaFinanceiraRequest.Nome,
                metaFinanceiraRequest.Descricao,
                metaFinanceiraRequest.ValorAlvo,
                metaFinanceiraRequest.ValorAtual,
                metaFinanceiraRequest.DataInicio,
                metaFinanceiraRequest.DataLimite,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar meta");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario, int idMetaFinanceira)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetaFinanceira.UpdateMetaFinanceira, new
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
                _notificationPool.AddNotification(500, "Erro ao atualizar meta financeira");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteMetaFinanceira(int idMetaFinanceira, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetaFinanceira.DeleteMetaFinanceira, new
            {
                IdUsuario = idUsuario,
                IdMetaFinanceira = idMetaFinanceira
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar meta financeira");
                return false;
            }

            return true;
        }
    }
}
