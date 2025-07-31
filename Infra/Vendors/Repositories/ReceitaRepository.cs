using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class ReceitaRepository : IReceitaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<ReceitaRepository> _logger;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public ReceitaRepository(ISqlServerConnectionHandler connectionHandler,
                                     NotificationPool notificationPool,
                                     ILogger<ReceitaRepository> logger)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        public async Task<List<ReceitaResponseInfra?>> GetReceitasPorUsuario(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<ReceitaResponseInfra>(
                SqlQueries.Receitas.GetReceitasPorIdUsuario, new
                {
                    IdUsuario = idUsuario,
                    Periodo = periodo
                }
            );

            var responseList = response.ToList();

            if (!responseList.Any())
            {
                _logger.LogWarning("Não foram encontradas receitas para o usuário {IdUsuario} no período {Periodo}.", idUsuario, periodo);
                _notificationPool.AddNotification(404, "Não foram encontradas receitas para o usuário no período informado");
            }

            return responseList;
        }

        public async Task<List<ReceitaPorCategoriaResponseInfra?>> GetReceitasPorCategoria(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<ReceitaPorCategoriaResponseInfra>(SqlQueries.Receitas.GetReceitasPorCategoria, new
            {
                IdUsuario = idUsuario,
                Periodo = periodo
            });

            var responseList = response.ToList();

            if (!responseList.Any())
            {
                _logger.LogWarning("Não foram encontradas receitas por categoria para o usuário {IdUsuario} no período {Periodo}.", idUsuario, periodo);
                _notificationPool.AddNotification(404, "Não foram encontradas receitas por categoria para o usuário no período informado");
            }

            return responseList;
        }

        public async Task<List<ReceitaPorContaResponseInfra?>> GetReceitasPorConta(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<ReceitaPorContaResponseInfra>(SqlQueries.Receitas.GetReceitasPorConta, new
            {
                IdUsuario = idUsuario,
                Periodo = periodo
            });

            var responseList = response.ToList();

            if (!responseList.Any())
            {
                _logger.LogWarning("Não foram encontradas receitas por conta para o usuário {IdUsuario} no período {Periodo}.", idUsuario, periodo);
                _notificationPool.AddNotification(404, "Não foram encontradas receitas por conta para o usuário no período informado");
            }

            return responseList;
        }

        public async Task<Decimal> GetReceitasTotalPorPeriodo(int idUsuario, int periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.ExecuteScalarAsync<Decimal>(SqlQueries.Receitas.GetTotalReceitasPeriodo, new
            {
                IdUsuario = idUsuario,
                Periodo = periodo
            });

            if (response <= 0)
            {
                _logger.LogWarning("Não foi encontrado total de receitas para o usuário {IdUsuario} no período {Periodo}.", idUsuario, periodo);
                _notificationPool.AddNotification(404, "Não foram encontradas receitas para o usuário no período informado");
            }

            return response;
        }


        public async Task<bool> InsertReceita(CadastrarReceitaRequestInfra receitaRequest, int idUsuario, int idCategoria, int idConta)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Receitas.InsertReceita, new
            {
                receitaRequest.Valor,
                receitaRequest.Descricao,
                receitaRequest.DataReceita,
                IdUsuario = idUsuario,
                IdConta = idConta,
                IdCategoria = idCategoria
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao cadastrar receita para o usuário {IdUsuario}. Categoria: {IdCategoria}, Conta: {IdConta}.", idUsuario, idCategoria, idConta);
                _notificationPool.AddNotification(500, "Erro ao cadastrar receita na base");
                return false;
            }

            _logger.LogInformation("Receita para o usuário {IdUsuario} cadastrada com sucesso. Categoria: {IdCategoria}, Conta: {IdConta}.", idUsuario, idCategoria, idConta);
            return true;
        }

        public async Task<bool> UpdateReceita(AtualizarReceitaRequestInfra receitaRequest, int idUsuario, int idReceita, int idCategoria, int idConta)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Receitas.UpdateReceita, new
            {
                receitaRequest.Valor,
                receitaRequest.Descricao,
                receitaRequest.DataReceita,
                IdConta = idConta,
                IdCategoria = idCategoria,
                IdUsuario = idUsuario,
                IdReceita = idReceita
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao atualizar receita {IdReceita} para o usuário {IdUsuario}.", idReceita, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao atualizar receita na base");
                return false;
            }

            _logger.LogInformation("Receita {IdReceita} do usuário {IdUsuario} atualizada com sucesso.", idReceita, idUsuario);
            return true;
        }

        public async Task<bool> DeleteReceita(int idUsuario, int idReceita)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Receitas.DeleteReceita, new
            {
                IdUsuario = idUsuario,
                IdReceita = idReceita
            });

            if (linhasAfetadas == 0)
            {
                _logger.LogWarning("Tentativa de exclusão falhou. Receita {IdReceita} do usuário {IdUsuario} não encontrada.", idReceita, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao deletar receita");
                return false;
            }

            _logger.LogInformation("Receita {IdReceita} do usuário {IdUsuario} excluída com sucesso.", idReceita, idUsuario);
            return true;
        }

    }
}