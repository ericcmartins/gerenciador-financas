using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<CategoriaRepository> _logger;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public CategoriaRepository(ISqlServerConnectionHandler connectionHandler,
                                      NotificationPool notificationPool,
                                      ILogger<CategoriaRepository> logger)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        public async Task<List<CategoriaResponseInfra>?> GetCategoriasPorUsuario(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var response = await connection.QueryAsync<CategoriaResponseInfra>(SqlQueries.Categorias.GetCategorias, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
            {
                _logger.LogWarning("Não foram encontradas categorias para o usuário {IdUsuario}.", idUsuario);
                _notificationPool.AddNotification(404, "Não foram encontradas categorias para o usuário");
            }

            return responseList;
        }

        public async Task<bool> InsertCategoria(CadastrarCategoriaRequestInfra categoriaRequest, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Categorias.InsertCategoria, new
            {
                categoriaRequest.Nome,
                categoriaRequest.Descricao,
                categoriaRequest.Tipo,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao cadastrar categoria '{NomeCategoria}' para o usuário {IdUsuario}.", categoriaRequest.Nome, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao cadastrar categoria");
                return false;
            }

            _logger.LogInformation("Categoria '{NomeCategoria}' para o usuário {IdUsuario} cadastrada com sucesso.", categoriaRequest.Nome, idUsuario);
            return true;
        }

        public async Task<bool> InsertCategoriasPadraoParaUsuario(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Categorias.InserirCategoriasPadrao, new { idUsuario });

            if (linhasAfetadas < 1)
            {
                _logger.LogError("Erro ao cadastrar categorias padrão para o usuário {IdUsuario}.", idUsuario);
                _notificationPool.AddNotification(500, "Erro ao cadastrar categorias padrão para o usuário");
                return false;
            }

            _logger.LogInformation("Categorias padrão para o usuário {IdUsuario} cadastradas com sucesso. Linhas afetadas: {LinhasAfetadas}", idUsuario, linhasAfetadas);
            return true;
        }
        public async Task<bool> UpdateCategoria(AtualizarCategoriaRequestInfra categoriaRequest, int idCategoria, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Categorias.UpdateCategoria, new
            {
                categoriaRequest.Nome,
                categoriaRequest.Descricao,
                categoriaRequest.Tipo,
                IdCategoria = idCategoria,
                IdUsuario = idUsuario
            });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao atualizar categoria {IdCategoria} para o usuário {IdUsuario}.", idCategoria, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao atualizar categoria");
                return false;
            }

            _logger.LogInformation("Categoria {IdCategoria} do usuário {IdUsuario} atualizada com sucesso.", idCategoria, idUsuario);
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
                _logger.LogWarning("Tentativa de exclusão falhou. Categoria {IdCategoria} do usuário {IdUsuario} não encontrada.", idCategoria, idUsuario);
                _notificationPool.AddNotification(500, "Erro ao deletar categoria");
                return false;
            }

            _logger.LogInformation("Categoria {IdCategoria} do usuário {IdUsuario} excluída com sucesso.", idCategoria, idUsuario);
            return true;
        }
    }
}