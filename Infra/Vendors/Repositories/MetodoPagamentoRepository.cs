using System.Data;
using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
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
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT IdMetodo, Nome, Descricao, Limite, Tipo, IdUsuario, IdConta
                                 FROM MetodoPagamento
                                 WHERE IdUsuario = @IdUsuario";
            
            var response = await connection.QueryAsync<MetodoPagamentoResponseInfra>(instrucaoSql, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontrados métodos de pagamento para o usuário");

            return responseList;
        }

        public async Task<bool> InsertMetodoPagamento(MetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario, int idConta)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO MetodoPagamento (Nome, Descricao, Limite, Tipo, IdUsuario, IdConta)
                                 VALUES (@Nome, @Descricao, @Limite, @Tipo, @IdUsuario, @IdConta)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                metodoPagamentoRequest.Nome, metodoPagamentoRequest.Descricao,metodoPagamentoRequest.Limite,
                metodoPagamentoRequest.Tipo, IdUsuario = idUsuario, IdConta = idConta
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
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE MetodoPagamento
                         SET Nome = COALESCE(@Nome, Nome),
                             Descricao = COALESCE(@Descricao, Descricao),
                             Limite = COALESCE(@Limite, Limite),
                             Tipo = COALESCE(@Tipo, Tipo),
                             IdConta = @IdConta
                         WHERE IdUsuario = @IdUsuario
                           AND Nome = @Nome";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                metodoPagamentoRequest.Nome, metodoPagamentoRequest.Descricao, metodoPagamentoRequest.Limite,
                metodoPagamentoRequest.Tipo, IdConta = idConta, IdUsuario = idUsuario
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
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM MetodoPagamento
                                 WHERE IdUsuario = @IdUsuario
                                   AND IdMetodo = @IdMetodo";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
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
