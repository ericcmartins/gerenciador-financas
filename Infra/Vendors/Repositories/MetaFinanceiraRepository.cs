using System.Data;
using Azure;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
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
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT Descricao, ValorAlvo, ValorAtual, DataInicio, DataLimite, Concluida, IdUsuario
                                 FROM MetaFinanceira
                                 WHERE IdUsuario = @IdUsuario";

            var response = await connection.QueryAsync<MetaFinanceiraResponseInfra>(instrucaoSql, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas metas financeiras para o usuário");

            return responseList;
        }

        public async Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO MetaFinanceira (Descricao, ValorAlvo, ValorAtual, DataInicio, DataLimite, Concluida, IdUsuario)
                                 VALUES (@Descricao, @ValorAlvo, @ValorAtual, @DataInicio, @DataLimite, @Concluida, @IdUsuario)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                metaFinanceiraRequest.Descricao, metaFinanceiraRequest.ValorAlvo, metaFinanceiraRequest.ValorAtual,
                metaFinanceiraRequest.DataInicio, metaFinanceiraRequest.DataLimite, metaFinanceiraRequest.Concluida, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar meta");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraResponse, int idUsuario, int idMetaFinanceira)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE MetaFinanceira
                     SET Descricao = COALESCE(@Descricao, Descricao),
                         ValorAlvo = COALESCE(@ValorAlvo, ValorAlvo),
                         ValorAtual = COALESCE(@ValorAtual, ValorAtual),
                         DataInicio = COALESCE(@DataInicio, DataInicio),
                         DataLimite = COALESCE(@DataLimite, DataLimite),
                         Concluida = COALESCE(@Concluida, Concluida)
                     WHERE IdUsuario = @IdUsuario
                       AND IdMeta = @IdMeta";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                metaFinanceiraResponse.Descricao, metaFinanceiraResponse.ValorAlvo, metaFinanceiraResponse.ValorAtual,
                metaFinanceiraResponse.DataInicio, metaFinanceiraResponse.DataLimite, metaFinanceiraResponse.Concluida, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar meta");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteMetaFinanceira(int idMeta, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM MetaFinanceira
                                 WHERE IdUsuario = @IdUsuario
                                   AND IdMeta = @IdMeta";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                IdUsuario = idUsuario, IdMeta = idMeta
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar meta");
                return false;
            }

            return true;
        }
    }
}
