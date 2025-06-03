using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public CategoriaRepository(ISqlServerConnectionHandler connectionHandler,
                                   NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<IEnumerable<CategoriaResponseInfra>?> GetCategorias(int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT IdCategoria, Nome, Descricao, IdUsuario
                                 FROM Categoria
                                 WHERE IdUsuario = @idUsuario";

            return await connection.QueryAsync<CategoriaResponseInfra>(instrucaoSql, new { idUsuario });
        }

        public async Task<bool> InsertCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO Categoria (Nome, Descricao, IdUsuario)
                                 VALUES (@Nome, @Descricao, @IdUsuario)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                categoriaRequest.Nome,
                categoriaRequest.Descricao,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
                return false;

            return true;
        }

        public async Task<bool> UpdateCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE Categoria
                                 SET Nome = @Nome,
                                     Descricao = @Descricao
                                 WHERE IdCategoria = @IdCategoria
                                   AND IdUsuario = @IdUsuario";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                categoriaRequest.Nome,
                categoriaRequest.Descricao,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
                return false;

            return true;
        }

        public async Task<bool> DeleteCategoria(string nomeCategoria, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM Categoria
                                 WHERE Nome = @NomeCategoria
                                   AND IdUsuario = @IdUsuario";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                NomeCategoria = nomeCategoria,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
                return false;

            return true;
        }
    }
}
