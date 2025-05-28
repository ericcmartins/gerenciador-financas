using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
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

        public async Task<DadosPessoaisResponseInfra?> GetDadosPessoais(string cpf)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @"SELECT * FROM usuario WHERE cpf = @cpf";

            return await connection.QueryFirstOrDefaultAsync<DadosPessoaisResponseInfra>(instrucaoSql, new { cpf }); ;
            
        }

        //public async Task<bool> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais)
        //{
        //    using (var connection = _connectionHandler.CreateConnection())
        //    {
        //        var instrucaoSql = @"INSERT INTO usuario (nome, cpf, email, senha, data_nascimento, telefone)
        //                             VALUES (@nome, @cpf, @email, @senha, @data_nascimento, @telefone)";

        //        var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, dadosPessoais);

        //        if (linhasAfetadas != 1)
        //            return false;
                                
        //        return true;
        //    }
        //}

        public async Task<bool> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, string cpf)
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

                var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, dadosPessoais);

                if (linhasAfetadas != 1)
                    return false;

                return true;
            }
        }

        public async Task<bool> UpdateSenha(string cpf, string senha)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"UPDATE usuario SET senha = @senha WHERE cpf = @cpf";

                var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new { cpf, senha });

                if (linhasAfetadas != 1)
                    return false;

                return true;
            }
        }

        public async Task<bool> UpdateEmail(string cpf, string email)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"UPDATE usuario SET email = @email WHERE cpf = @cpf";

                var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new { cpf, email });

                if (linhasAfetadas != 1)
                    return false;

                return true;
            }
        }

        public async Task<bool> UpdateTelefone(string cpf, string telefone)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"UPDATE usuario SET telefone = @telefone WHERE cpf = @cpf";

                var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new { cpf, telefone });

                if (linhasAfetadas != 1)
                    return false;

                return true;
            }
        }

        public async Task<bool> DeleteConta(string cpf)
        {
            using (var connection = _connectionHandler.CreateConnection())
            {
                var instrucaoSql = @"DELETE FROM usuario WHERE cpf = @cpf";

                var linhasAfetadas = await connection.ExecuteAsync(instrucaoSql, new { cpf });

                if (linhasAfetadas != 1)
                    return false;

                return true;
            }
        }
    }
}
