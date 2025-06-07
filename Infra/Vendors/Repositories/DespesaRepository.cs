using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class DespesaRepository : IDespesaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public DespesaRepository(ISqlServerConnectionHandler connectionHandler,
                                         NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<DespesaResponseInfra?>> GetDespesas(int idUsuario, int? periodo)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT IdDespesa, Valor, Descricao, Data, Recorrente, Frequencia, 
                                IdUsuario, IdConta, IdCategoria, IdMetodoPagamento
                         FROM Despesa
                         WHERE IdUsuario = @IdUsuario";

            var response = await connection.QueryAsync<DespesaResponseInfra>(instrucaoSql, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas para o usuário");

            return responseList;
        }

        public async Task<bool> InsertDespesa(DespesaRequestInfra despesaRequest, int idUsuario, int idConta, int idCategoria, int idMetodoPagamento)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO Despesa (Valor, Descricao, Data, Recorrente, Frequencia, 
                                               IdUsuario, IdConta, IdCategoria, IdMetodoPagamento)
                         VALUES (@Valor, @Descricao, @Data, @Recorrente, @Frequencia, 
                                 @IdUsuario, @IdConta, @IdCategoria, @IdMetodoPagamento)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                despesaRequest.Valor,
                despesaRequest.Descricao,
                despesaRequest.Data,
                despesaRequest.Recorrente,
                despesaRequest.Frequencia,
                IdUsuario = idUsuario,
                IdConta = idConta,
                IdCategoria = idCategoria,
                IdMetodoPagamento = idMetodoPagamento
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar despesa");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateDespesa(DespesaRequestInfra despesaRequest, int idUsuario, int idDespesa, int idCategoria, int idConta, int idMetodoPagamento)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE Despesa
                         SET Valor = COALESCE(@Valor, Valor),
                             Descricao = COALESCE(@Descricao, Descricao),
                             Data = COALESCE(@Data, Data),
                             Recorrente = COALESCE(@Recorrente, Recorrente),
                             Frequencia = COALESCE(@Frequencia, Frequencia),
                             IdConta = COALESCE(@IdConta, IdConta),
                             IdCategoria = COALESCE(@IdCategoria, IdCategoria),
                             IdMetodoPagamento = COALESCE(@IdMetodoPagamento, IdMetodoPagamento)
                         WHERE IdUsuario = @IdUsuario
                           AND IdDespesa = @IdDespesa";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                despesaRequest.Valor,
                despesaRequest.Descricao,
                despesaRequest.Data,
                despesaRequest.Recorrente,
                despesaRequest.Frequencia,
                IdConta = idConta,
                IdCategoria = idCategoria,
                IdMetodoPagamento = idMetodoPagamento,
                IdUsuario = idUsuario,
                IdDespesa = idDespesa
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar despesa");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteDespesa(int idUsuario, int idDespesa)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM Despesa
                         WHERE IdUsuario = @IdUsuario
                           AND IdDespesa = @IdDespesa";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                IdUsuario = idUsuario,
                IdDespesa = idDespesa
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar despesa");
                return false;
            }

            return true;
        }
    }
}