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

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.QueryAsync<DespesaResponseInfra>(
                SqlQueries.Despesa.GetDespesasPorUsuario,
                new
                {
                    IdUsuario = idUsuario,
                    DataInicio = dataInicio,
                    DataFim = dataFim
                },
                commandTimeout: 120
            );

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas no período informado para o usuário");

            return responseList;
        }

        public async Task<List<DespesaPorCategoriaResponseInfra?>> GetDespesasPorCategoria(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.QueryAsync<DespesaPorCategoriaResponseInfra>(SqlQueries.Despesa.GetDespesasPorCategoria, new
            {
                IdUsuario = idUsuario,
                DataInicio = dataInicio,
                DataFim = dataFim
            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por categoria para o usuário no período informado");

            return responseList;
        }

        public async Task<List<DespesaPorContaResponseInfra?>> GetDespesasPorConta(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.QueryAsync<DespesaPorContaResponseInfra>(SqlQueries.Despesa.GetDespesasPorConta, new
            {
                IdUsuario = idUsuario,
                DataInicio = dataInicio,
                DataFim = dataFim
            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por conta para o usuário no período informado");

            return responseList;
        }

        public async Task<List<DespesaPorMetodoPagamentoResponseInfra?>> GetDespesasPorMetodoPagamento(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.QueryAsync<DespesaPorMetodoPagamentoResponseInfra>(SqlQueries.Despesa.GetDespesasPorMetodoPagamento, new
            {
                IdUsuario = idUsuario,
                DataInicio = dataInicio,
                DataFim = dataFim
            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por método de pagamento para o usuário no período informado");

            return responseList;
        }

        public async Task<decimal> GetTotalDespesasPeriodo(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.ExecuteScalarAsync<decimal>(SqlQueries.Despesa.GetTotalDespesasNoPeriodo, new
            {
                IdUsuario = idUsuario,
                DataInicio = dataInicio,
                DataFim = dataFim
            });

            if (response <= 0)
                _notificationPool.AddNotification(404, "Não foram encontradas despesas no período informado para o usuário");

            return response;
        }

        public async Task<bool> InsertDespesa(DespesaRequestInfra despesaRequest, int idUsuario, int idCategoria, int idConta, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Despesa.InsertDespesa, new
            {
                despesaRequest.Valor,
                despesaRequest.Descricao,
                despesaRequest.DataDespesa,
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
                despesaRequest.DataDespesa,
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

            if (linhasAfetadas != -1)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar despesa");
                return false;
            }

            return true;
        }

    }
}