using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class MetodoPagamentoRepository : IMetodoPagamentoRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public MetodoPagamentoRepository(ISqlServerConnectionHandler connectionHandler,
                                         NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<MetodoPagamentoResponseInfra?>> GetMetodosPagamentoUsuario(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var response = await connection.QueryAsync<MetodoPagamentoResponseInfra>(SqlQueries.MetodoPagamento.GetMetodosPagamentoUsuario, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontrados métodos de pagamento para o usuário");

            return responseList;
        }

        public async Task<bool> InsertMetodoPagamento(MetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario, int idConta)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetodoPagamento.InsertMetodoPagamento, new
            {
                metodoPagamentoRequest.Nome,
                metodoPagamentoRequest.Descricao,
                metodoPagamentoRequest.Limite,
                metodoPagamentoRequest.Tipo,
                IdUsuario = idUsuario,
                IdConta = idConta
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar método de pagamento");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateMetodoPagamento(MetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario, int idConta, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetodoPagamento.UpdateMetodoPagamento, new
            {
                metodoPagamentoRequest.Nome,
                metodoPagamentoRequest.Descricao,
                metodoPagamentoRequest.Limite,
                metodoPagamentoRequest.Tipo,
                IdConta = idConta,
                IdUsuario = idUsuario,
                IdMetodo = idMetodoPagamento
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar método de pagamento");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MetodoPagamento.DeleteMetodoPagamento, new
            {
                IdUsuario = idUsuario,
                IdMetodo = idMetodoPagamento
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar método de pagamento");
                return false;
            }

            return true;
        }
    }
}
