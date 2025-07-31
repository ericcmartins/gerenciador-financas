using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class DespesaRepository : IDespesaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<DespesaRepository> _logger;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public DespesaRepository(ISqlServerConnectionHandler connectionHandler,
                                     NotificationPool notificationPool,
                                     ILogger<DespesaRepository> logger)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
            _logger = logger;
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
            {
                _logger.LogWarning("Não foram encontradas despesas no período {Periodo} para o usuário {IdUsuario}.", periodo, idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas despesas no período informado para o usuário");
            }

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
            {
                _logger.LogWarning("Não foram encontradas despesas por categoria no período {Periodo} para o usuário {IdUsuario}.", periodo, idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por categoria para o usuário no período informado");
            }

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
            {
                _logger.LogWarning("Não foram encontradas despesas por conta no período {Periodo} para o usuário {IdUsuario}.", periodo, idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por conta para o usuário no período informado");
            }

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
            {
                _logger.LogWarning("Não foram encontradas despesas por método de pagamento no período {Periodo} para o usuário {IdUsuario}.", periodo, idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas despesas por método de pagamento para o usuário no período informado");
            }

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
            {
                _logger.LogWarning("Não foi encontrado total de despesas no período {Periodo} para o usuário {IdUsuario}.", periodo, idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas despesas no período informado para o usuário");
            }

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
                _logger.LogError("Erro ao cadastrar despesa para o usuário {IdUsuario}. Categoria: {IdCategoria}, Método de Pagamento: {IdMetodoPagamento}.", idUsuario, idCategoria, idMetodoPagamento);
                _notificationPool.AddNotification(500, "Erro ao cadastrar despesa na base");
                return false;
            }

            _logger.LogInformation("Despesa para o usuário {IdUsuario} cadastrada com sucesso.", idUsuario);
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
                _logger.LogError("Erro ao atualizar despesa {IdDespesa} para o usuário {IdUsuario}.", idDespesa, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao atualizar despesa na base");
                return false;
            }

            _logger.LogInformation("Despesa {IdDespesa} do usuário {IdUsuario} atualizada com sucesso.", idDespesa, idUsuario);
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
                _logger.LogWarning("Tentativa de exclusão falhou. Despesa {IdDespesa} do usuário {IdUsuario} não encontrada.", idDespesa, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao deletar despesa");
                return false;
            }

            _logger.LogInformation("Despesa {IdDespesa} do usuário {IdUsuario} excluída com sucesso.", idDespesa, idUsuario);
            return true;
        }

    }
}