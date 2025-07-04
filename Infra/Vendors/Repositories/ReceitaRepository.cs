using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class ReceitaRepository : IReceitaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public ReceitaRepository(ISqlServerConnectionHandler connectionHandler,
                                     NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<ReceitaResponseInfra?>> GetReceitas(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.QueryAsync<ReceitaResponseInfra>(
                SqlQueries.Receita.GetReceitasPorId, new
                {
                    IdUsuario = idUsuario,
                    DataInicio = dataInicio,
                    DataFim = dataFim
                }
            );

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas receitas no período informado para o usuário");

            return responseList;
        }

        public async Task<List<ReceitaPorCategoriaResponseInfra?>> GetReceitasPorCategoria(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.QueryAsync<ReceitaPorCategoriaResponseInfra>(SqlQueries.Receita.GetReceitasPorCategoria, new
            {
                IdUsuario = idUsuario,
                DataInicio = dataInicio,
                DataFim = dataFim
            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas receitas por categoria para o usuário no período informado");

            return responseList;
        }

        public async Task<List<ReceitaPorContaResponseInfra?>> GetReceitasPorConta(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.QueryAsync<ReceitaPorContaResponseInfra>(SqlQueries.Receita.GetReceitasPorConta, new
            {
                IdUsuario = idUsuario,
                DataInicio = dataInicio,
                DataFim = dataFim
            });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas receitas por conta para o usuário no período informado");

            return responseList;
        }

        public async Task<Decimal> GetReceitasTotalPorPeriodo(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (periodo.HasValue)
            {
                dataInicio = DateTime.Today.AddDays(-periodo.Value);
                dataFim = DateTime.Today.AddDays(1).AddTicks(-1);
            }

            var response = await connection.ExecuteScalarAsync<Decimal>(SqlQueries.Receita.GetTotalReceitasPeriodo, new
            {
                IdUsuario = idUsuario,
                DataInicio = dataInicio,
                DataFim = dataFim
            });

            if (response <= 0)
            {
                _notificationPool.AddNotification(404, "Não foram encontradas receitas no período informado para o usuário");
            }

            return response;
        }


        public async Task<bool> InsertReceita(ReceitaRequestInfra receitaRequest, int idUsuario, int idConta, int idCategoria)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Receita.InsertReceita, new
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
                _notificationPool.AddNotification(500, "Erro ao cadastrar receita");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateReceita(ReceitaRequestInfra receitaRequest, int idUsuario, int idReceita, int idCategoria, int idConta)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Receita.UpdateReceita, new
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
                _notificationPool.AddNotification(500, "Erro ao atualizar receita");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteReceita(int idUsuario, int idReceita)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Receita.DeleteReceita, new
            {
                IdUsuario = idUsuario,
                IdReceita = idReceita
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar receita");
                return false;
            }

            return true;
        }

    }
}