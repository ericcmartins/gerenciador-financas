using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
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
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var response = await connection.QueryAsync<CategoriaResponseInfra>(SqlQueries.Categorias.GetCategorias, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas categorias para o usuário");

            return responseList;
        }

        public async Task<bool> InsertCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Categorias.InsertCategoria, new
            {
                categoriaRequest.Nome,
                categoriaRequest.Descricao,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar categoria");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateCategoria(CategoriaRequestInfra categoriaRequest, int idCategoria, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Categorias.UpdateCategoria, new
            {
                categoriaRequest.Nome,
                categoriaRequest.Descricao,
                IdCategoria = idCategoria,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar categoria");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteCategoria(int idCategoria, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Categorias.DeleteCategoria, new
            {
                IdCategoria = idCategoria,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas == 0)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar categoria");
                return false;
            }

            return true;
        }
    }
}
