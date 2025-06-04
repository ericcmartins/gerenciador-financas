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

        public async Task<List<CategoriaResponseInfra>?> GetCategorias(int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT IdCategoria, Nome, Descricao, IdUsuario
                                 FROM Categoria
                                 WHERE IdUsuario = @idUsuario";

            var response = await connection.QueryAsync<CategoriaResponseInfra>(instrucaoSql, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas categorias para o usuário");

            return responseList;
        }

        public async Task<bool> InsertCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO Categoria (Nome, Descricao, IdUsuario)
                                 VALUES (@Nome, @Descricao, @IdUsuario)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                categoriaRequest.Nome, categoriaRequest.Descricao, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar categoria");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario) //ver se troco para id depois
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE Categoria
                     SET Nome = COALESCE(@Nome, Nome),
                         Descricao = COALESCE(@Descricao, Descricao)
                     WHERE Nome = @NomeFiltro
                       AND IdUsuario = @IdUsuario";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                categoriaRequest.Nome, categoriaRequest.Descricao, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar categoria");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteCategoria(string nomeCategoria, int idUsuario) //ver se troco pro id também
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM Categoria
                                 WHERE Nome = @NomeCategoria
                                   AND IdUsuario = @IdUsuario";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                NomeCategoria = nomeCategoria, IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar categoria");
                return false;
            }

            return true;
        }
    }
}
