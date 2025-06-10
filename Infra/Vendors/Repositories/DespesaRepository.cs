using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
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
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var response = await connection.QueryAsync<DespesaResponseInfra>(SqlQueries.Despesa.GetDespesas, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas para o usuário");

            return responseList;
        }

        public async Task<bool> InsertDespesa(DespesaRequestInfra despesaRequest, int idUsuario, int idConta, int idCategoria, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Despesa.InsertDespesa, new
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
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Despesa.UpdateDespesa, new
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
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Despesa.DeleteDespesa, new
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