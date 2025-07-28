using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;
using System.Net;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Extensions.Logging;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<UsuarioRepository> _logger;

        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public UsuarioRepository(ISqlServerConnectionHandler connectionHandler,
                                 NotificationPool notificationPool,
                                 ILogger<UsuarioRepository> logger)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        public async Task<DadosPessoaisResponseInfra?> GetDadosPessoais(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryFirstOrDefaultAsync<DadosPessoaisResponseInfra>(SqlQueries.Usuarios.GetDadosPessoais, new { idUsuario });

            if (response == null)
            {
                _logger.LogWarning("Usuário com ID {IdUsuario} não foi encontrado.", idUsuario);
                _notificationPool.AddNotification(404, "Usuario não encontrado");
            }

            return response;
        }

        public async Task<bool> InsertCadastroUsuario(CadastrarUsuarioRequestInfra dadosCadastro)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();
            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Usuarios.InsertDadosPessoais,
                new
                {
                    dadosCadastro.Nome,
                    dadosCadastro.Email,
                    dadosCadastro.SenhaHash,
                    dadosCadastro.RoleUsuario,
                    dadosCadastro.DataNascimento,
                    dadosCadastro.Telefone
                });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao cadastrar usuário com email: {Email}", dadosCadastro.Email);
                _notificationPool.AddNotification(500, "Erro ao cadastrar o usuário");
                return false;
            }

            _logger.LogInformation("Usuário com email {Email} cadastrado com sucesso.", dadosCadastro.Email);
            return true;
        }

        public async Task<bool> UpdateDadosPessoais(AtualizarDadosCadastraisRequestInfra dadosPessoais, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Usuarios.UpdateDadosPessoais,
                new
                {
                    dadosPessoais.Nome,
                    dadosPessoais.Email,
                    dadosPessoais.DataNascimento,
                    dadosPessoais.Telefone,
                    idUsuario
                });

            if (linhasAfetadas != 1)
            {
                _logger.LogError("Erro ao atualizar usuário {IdUsuario}", idUsuario);
                _notificationPool.AddNotification(500, "Erro ao atualizar as informações do usuário");
                return false;
            }

            _logger.LogInformation("Dados do usuário {IdUsuario} atualizados com sucesso.", idUsuario);
            return true;
        }

        public async Task<bool> DeleteConta(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Usuarios.DeleteUsuario, new { idUsuario });
            if (linhasAfetadas == 0)
            {
                _logger.LogWarning("Tentativa de exclusão falhou. Usuário {IdUsuario} não encontrado.", idUsuario);
                _notificationPool.AddNotification(404, "Usuário não encontrado");
                return false;
            }

            _logger.LogInformation("Usuário {IdUsuario} excluído com sucesso.", idUsuario);
            return true;
        }

        public async Task<DadosPessoaisResponseInfra?> GetUsuarioPorEmail(string email)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var usuario = await connection.QueryFirstOrDefaultAsync<DadosPessoaisResponseInfra>(SqlQueries.Usuarios.GetDadosPessoaisPorEmail, new { Email = email });

            return usuario;
        }
    }
}
