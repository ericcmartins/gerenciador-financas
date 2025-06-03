using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;
using System.Net;

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
            using var connection = _connectionHandler.CreateConnection();
            var instrucaoSql = @"SELECT IdUsuario, Nome, Email, Senha, DataNascimento, Telefone 
                                FROM Usuario 
                                WHERE IdUsuario = @idUsuario";

            var response = await connection.QueryFirstOrDefaultAsync<DadosPessoaisResponseInfra>(instrucaoSql, new { idUsuario });

            if (response == null)
                _notificationPool.AddNotification(404, "Usuario não encontrado");

            return response;
        }

        public async Task<bool> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"INSERT INTO Usuario (Nome, Email, Senha, DataNascimento, Telefone)
                                   VALUES (@Nome, @Email, @Senha, @DataNascimento, @Telefone)";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, dadosPessoais);
            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar o usuário");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, int idUsuario)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"UPDATE Usuario 
                                    SET Nome = COALESCE(@Nome, Nome),
                                        Email = COALESCE(@Email, Email),
                                        Senha = COALESCE(@Senha, Senha),
                                        DataNascimento = COALESCE(@DataNascimento, DataNascimento),
                                        Telefone = COALESCE(@Telefone, Telefone)
                                    WHERE IdUsuario = @idUsuario";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new
            {
                dadosPessoais.Nome,
                dadosPessoais.Email,
                dadosPessoais.Senha,
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
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"DELETE FROM Usuario WHERE IdUsuario = @idUsuario";

            var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new { idUsuario });
            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(404, "Usuário não encontrado");
                return false;
            }

            return true;
        }
    }
}
