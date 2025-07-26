using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public PagamentoRepository(ISqlServerConnectionHandler connectionHandler,
                                         NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<MetodoPagamentoResponseInfra?>> GetMetodosPagamentoUsuario(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<MetodoPagamentoResponseInfra>(SqlQueries.MetodosPagamento.GetMetodosPagamentoUsuario, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontrados métodos de pagamento para o usuário na base");

            return responseList;
        }

        public async Task<bool> InsertMetodoPagamento(CadastrarMetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario, int idConta)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetodosPagamento.InsertMetodoPagamento, new
            {
                metodoPagamentoRequest.Nome,
                metodoPagamentoRequest.IdTipoMetodo,
                metodoPagamentoRequest.Limite,
                IdUsuario = idUsuario,
                IdConta = idConta
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar método de pagamento na base");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateMetodoPagamento(AtualizarMetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario, int idConta, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetodosPagamento.UpdateMetodoPagamento, new
            {
                metodoPagamentoRequest.Nome,
                metodoPagamentoRequest.IdTipoMetodo,
                metodoPagamentoRequest.Limite,
                IdConta = idConta,
                IdUsuario = idUsuario,
                IdMetodo = idMetodoPagamento
            });

            if (linhasAfetadas < 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar método de pagamento na base");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetodosPagamento.DeleteMetodoPagamento, new
            {
                IdUsuario = idUsuario,
                IdMetodo = idMetodoPagamento
            });

            if (linhasAfetadas == 0)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar método de pagamento da base");
                return false;
            }

            return true;
        }
    }
}
