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

        public async Task<List<ReceitaResponseInfra?>> GetReceita(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var response = await connection.QueryAsync<ReceitaResponseInfra>(SqlQueries.Receita.GetReceitas, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas receitas para o usuário");

            return responseList;
        }

        public async Task<bool> InsertReceita(ReceitaRequestInfra receitaRequest, int idUsuario, int idConta, int idCategoria)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Receita.InsertReceita, new
            {
                receitaRequest.Valor,
                receitaRequest.Descricao,
                receitaRequest.Data,
                receitaRequest.Recorrente,
                receitaRequest.Frequencia,
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
                receitaRequest.Data,
                receitaRequest.Recorrente,
                receitaRequest.Frequencia,
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
