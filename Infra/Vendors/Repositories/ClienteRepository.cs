using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;

        public ClienteRepository(ISqlServerConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }

        public async Task<TResultInfra<DadosPessoaisResponseInfra>> GetDadosPessoais(string cpf)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"SELECT * FROM usuario WHERE cpf = @cpf";

                var response = connection.QueryFirstOrDefault<DadosPessoaisResponseInfra>(instrucaoSql, new { cpf });

                if (response is null)
                {
                    return TResultInfra<DadosPessoaisResponseInfra>.Fail(ErrosInfra.NotFound, "Usuário não encontrado.");
                }

                return TResultInfra<DadosPessoaisResponseInfra>.Ok(response);
            }
        }

        public async Task<TResultInfra<string>> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"INSERT INTO usuario (nome, cpf, email, senha, data_nascimento, telefone)
                                     VALUES (@nome, @cpf, @email, @senha, @data_nascimento, @telefone)";

                var linhasAfetadas = connection.Execute(instrucaoSql, dadosPessoais);

                if (linhasAfetadas == 1)
                {
                    return TResultInfra<string>.Ok("Cadastro realizado com sucesso");
                }

                return TResultInfra<string>.Fail(ErrosInfra.DatabaseError, "Falha ao realizar o cadastro.");
            }
        }

        public async Task<TResultInfra<string>> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, string cpf)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"UPDATE usuario 
                                     SET nome = @nome,
                                         email = @email,
                                         senha = @senha,
                                         data_nascimento = @dataNascimento,
                                         telefone = @telefone
                                     WHERE cpf = @cpf";

                var linhasAfetadas = connection.Execute(instrucaoSql, dadosPessoais);

                if (linhasAfetadas == 1)
                {
                    return TResultInfra<string>.Ok("Atualização realizada com sucesso");
                }

                return TResultInfra<string>.Fail(ErrosInfra.NotFound, "Usuário não encontrado para atualização.");
            }
        }

        public async Task<TResultInfra<string>> UpdateSenha(string cpf, string senha)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"UPDATE usuario SET senha = @senha WHERE cpf = @cpf";

                var linhasAfetadas = connection.Execute(instrucaoSql, new { cpf, senha });

                if (linhasAfetadas == 1)
                {
                    return TResultInfra<string>.Ok("Update de senha realizado com sucesso");
                }

                return TResultInfra<string>.Fail(ErrosInfra.NotFound, "Usuário não encontrado para atualização de senha.");
            }
        }

        public async Task<TResultInfra<string>> UpdateEmail(string cpf, string email)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"UPDATE usuario SET email = @email WHERE cpf = @cpf";

                var linhasAfetadas = connection.Execute(instrucaoSql, new { cpf, email });

                if (linhasAfetadas == 1)
                {
                    return TResultInfra<string>.Ok("Update de email realizado com sucesso");
                }

                return TResultInfra<string>.Fail(ErrosInfra.NotFound, "Usuário não encontrado para atualização de email.");
            }
        }

        public async Task<TResultInfra<string>> UpdateTelefone(string cpf, string telefone)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"UPDATE usuario SET telefone = @telefone WHERE cpf = @cpf";

                var linhasAfetadas = connection.Execute(instrucaoSql, new { cpf, telefone });

                if (linhasAfetadas == 1)
                {
                    return TResultInfra<string>.Ok("Update de telefone realizado com sucesso");
                }

                return TResultInfra<string>.Fail(ErrosInfra.NotFound, "Usuário não encontrado para atualização de telefone.");
            }
        }

        public async Task<TResultInfra<string>> DeleteConta(string cpf)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"DELETE FROM usuario WHERE cpf = @cpf";

                var linhasAfetadas = connection.Execute(instrucaoSql, new { cpf });

                if (linhasAfetadas == 1)
                {
                    return TResultInfra<string>.Ok("Delete realizado com sucesso");
                }

                return TResultInfra<string>.Fail(ErrosInfra.NotFound, "Usuário não encontrado para exclusão.");
            }
        }
    }
}
