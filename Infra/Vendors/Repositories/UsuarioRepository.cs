using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;
using System.Net;
using gerenciador.financas.Infra.Vendors.Queries;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public UsuarioRepository(ISqlServerConnectionHandler connectionHandler,
                                 NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<DadosPessoaisResponseInfra?> GetDadosPessoais(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryFirstOrDefaultAsync<DadosPessoaisResponseInfra>(SqlQueries.Usuario.GetDadosPessoais, new { idUsuario });

            if (response == null)
                _notificationPool.AddNotification(404, "Usuario não encontrado");

            return response;
        }

        public async Task<bool> InsertCadastroUsuario(CadastrarUsuarioRequestInfra dadosCadastro)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();
            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Usuario.InsertDadosPessoais,
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
                _notificationPool.AddNotification(500, "Erro ao cadastrar o usuário");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateDadosPessoais(AtualizarDadosCadastraisRequestInfra dadosPessoais, int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Usuario.UpdateDadosPessoais,
                new
                {
                    dadosPessoais.Nome,
                    dadosPessoais.Email,
                    dadosPessoais.SenhaHash,
                    dadosPessoais.DataNascimento,
                    dadosPessoais.Telefone,
                    idUsuario
                });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar as informações do usuário");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteConta(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Usuario.DeleteUsuario, new { idUsuario });
            if (linhasAfetadas == 0)
            {
                _notificationPool.AddNotification(404, "Usuário não encontrado");
                return false;
            }

            return true;
        }

        public async Task<DadosPessoaisResponseInfra?> GetUsuarioPorEmail(string email)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var usuario = await connection.QueryFirstOrDefaultAsync<DadosPessoaisResponseInfra>(SqlQueries.Usuario.GetDadosPessoaisPorEmail, new { Email = email });

            if (usuario == null)
                _notificationPool.AddNotification(404, "Usuário não encontrado");

            return usuario;
        }
    }
}
