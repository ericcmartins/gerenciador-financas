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
        public async Task<List<DespesaResponseInfra?>> GetDespesasPorUsuario(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<DespesaResponseInfra>(
                SqlQueries.Despesas.GetDespesasPorUsuario,
                new
                {
                    IdUsuario = idUsuario,
                    Periodo = periodo
                },
                commandTimeout: 120
            );

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas no período informado para o usuário");

            return responseList;
        }

        public async Task<List<DespesaPorCategoriaResponseInfra?>> GetDespesasPorCategoria(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<DespesaPorCategoriaResponseInfra>(SqlQueries.Despesas.GetDespesasPorCategoria, new
            {
                IdUsuario = idUsuario,
                Periodo = periodo
            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por categoria para o usuário no período informado");

            return responseList;
        }

        public async Task<List<DespesaPorContaResponseInfra?>> GetDespesasPorConta(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<DespesaPorContaResponseInfra>(SqlQueries.Despesas.GetDespesasPorConta, new
            {
                IdUsuario = idUsuario,
                Periodo = periodo

            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por conta para o usuário no período informado");

            return responseList;
        }

        public async Task<List<DespesaPorMetodoPagamentoResponseInfra?>> GetDespesasPorMetodoPagamento(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<DespesaPorMetodoPagamentoResponseInfra>(SqlQueries.Despesas.GetDespesasPorMetodoPagamento, new
            {
                IdUsuario = idUsuario,
                Pe = periodo
            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por método de pagamento para o usuário no período informado");

            return responseList;
        }

        public async Task<decimal> GetTotalDespesasPeriodo(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.ExecuteScalarAsync<decimal>(SqlQueries.Despesas.GetTotalDespesasNoPeriodo, new
            {
                IdUsuario = idUsuario,
                Periodo = periodo
            });

            if (response <= 0)
                _notificationPool.AddNotification(404, "Não foram encontradas despesas no período informado para o usuário");

            return response;
        }

        public async Task<bool> InsertDespesa(CadastrarDespesaRequestInfra despesaRequest, int idUsuario, int? idCategoria, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Despesas.InsertDespesa, new
            {
                despesaRequest.Valor,
                despesaRequest.Descricao,
                despesaRequest.DataDespesa,
                IdUsuario = idUsuario,
                IdCategoria = idCategoria,
                IdMetodoPagamento = idMetodoPagamento
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar despesa na base");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateDespesa(AtualizarDespesaRequestInfra despesaRequest, int idUsuario, int idDespesa, int? idCategoria, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Despesas.UpdateDespesa, new
            {
                despesaRequest.Valor,
                despesaRequest.Descricao,
                despesaRequest.DataDespesa,
                IdCategoria = idCategoria,
                IdMetodoPagamento = idMetodoPagamento,
                IdUsuario = idUsuario,
                IdDespesa = idDespesa
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar despesa na base");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteDespesa(int idUsuario, int idDespesa)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Despesas.DeleteDespesa, new
            {
                IdUsuario = idUsuario,
                IdDespesa = idDespesa
            });

            if (linhasAfetadas == 0)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar despesa");
                return false;
            }

            return true;
        }

    }
}