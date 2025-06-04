using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
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

        public Task<List<DespesaResponseInfra?>> GetDespesas(int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT IdReceita, Valor, Descricao, Data, Recorrente, Frequencia, 
                                FkUsuarioIdUsuario, FkContaIdConta, FkCategoriaUsuarioIdCategoria
                         FROM Receita
                         WHERE FkUsuarioIdUsuario = @IdUsuario";

            var response = await connection.QueryAsync<ReceitaResponseInfra>(instrucaoSql, new { IdUsuario = idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas receitas para o usuário");

            return responseList;
        }


        public Task<bool> InsertDespesa(DespesaRequestInfra receitaRequest, int idUsuario, int idConta, int idCategoria, int idMetodoPagamento)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO Receita (Valor, Descricao, Data, Recorrente, Frequencia, 
                                               FkUsuarioIdUsuario, FkContaIdConta, FkCategoriaUsuarioIdCategoria)
                         VALUES (@Valor, @Descricao, @Data, @Recorrente, @Frequencia, 
                                 @FkUsuarioIdUsuario, @FkContaIdConta, @FkCategoriaUsuarioIdCategoria)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                receitaRequest.Valor,
                receitaRequest.Descricao,
                receitaRequest.Data,
                receitaRequest.Recorrente,
                receitaRequest.Frequencia,
                FkUsuarioIdUsuario = idUsuario,
                FkContaIdConta = idConta,
                FkCategoriaUsuarioIdCategoria = idCategoria
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar receita");
                return false;
            }

            return true;
        }


        public Task<bool> UpdateDespesa(DespesaRequestInfra receitaRequest, int idUsuario, int idDespesa, int idCategoria, int idConta, int idMetodoPagamento)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE Receita
                         SET Valor = COALESCE(@Valor, Valor),
                             Descricao = COALESCE(@Descricao, Descricao),
                             Data = COALESCE(@Data, Data),
                             Recorrente = COALESCE(@Recorrente, Recorrente),
                             Frequencia = COALESCE(@Frequencia, Frequencia),
                             FkContaIdConta = COALESCE(@FkContaIdConta, FkContaIdConta),
                             FkCategoriaUsuarioIdCategoria = COALESCE(@FkCategoriaUsuarioIdCategoria, FkCategoriaUsuarioIdCategoria)
                         WHERE FkUsuarioIdUsuario = @FkUsuarioIdUsuario
                           AND IdReceita = @IdReceita";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                receitaRequest.Valor,
                receitaRequest.Descricao,
                receitaRequest.Data,
                receitaRequest.Recorrente,
                receitaRequest.Frequencia,
                FkContaIdConta = idConta,
                FkCategoriaUsuarioIdCategoria = idCategoria,
                FkUsuarioIdUsuario = idUsuario,
                IdReceita = idReceita
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar receita");
                return false;
            }

            return true;
        }

        public Task<bool> DeleteDespesa(int idUsuario, int idDespesa)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM Receita
                         WHERE FkUsuarioIdUsuario = @FkUsuarioIdUsuario
                           AND IdReceita = @IdReceita";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                FkUsuarioIdUsuario = idUsuario,
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
