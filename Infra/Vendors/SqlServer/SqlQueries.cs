using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Queries
{
    public static class SqlQueries
    {
        #region Categoria
        public static class Categoria
        {
            public const string GetCategorias = @"
                SELECT IdCategoria, Nome, Descricao, IdUsuario
                FROM Categoria
                WHERE IdUsuario = @idUsuario";

            public const string InsertCategoria = @"
                INSERT INTO Categoria (Nome, Descricao, IdUsuario)
                VALUES (@Nome, @Descricao, @IdUsuario)";

            public const string UpdateCategoria = @"
                UPDATE Categoria
                SET Nome = COALESCE(@Nome, Nome),
                    Descricao = COALESCE(@Descricao, Descricao)
                WHERE IdCategoria = @IdCategoria
                  AND IdUsuario = @IdUsuario";

            public const string DeleteCategoria = @"
                DELETE FROM Categoria
                WHERE IdCategoria = @IdCategoria
                  AND IdUsuario = @IdUsuario";
        }
        #endregion

        #region Conta
        public static class Conta
        {
            public const string GetContas = @"
                SELECT IdConta, NumeroConta, Tipo, Instituicao, IdUsuario
                FROM Conta
                WHERE IdUsuario = @idUsuario";

            public const string InsertConta = @"
                INSERT INTO Conta (NumeroConta, Tipo, Instituicao, IdUsuario)
                VALUES (@NumeroConta, @Tipo, @Instituicao, @IdUsuario)";

            public const string UpdateConta = @"
                UPDATE Conta
                SET NumeroConta = COALESCE(@NumeroConta, NumeroConta),
                    Tipo = COALESCE(@Tipo, Tipo),
                    Instituicao = COALESCE(@Instituicao, Instituicao)
                WHERE IdConta = @IdConta
                  AND IdUsuario = @IdUsuario";

            public const string DeleteConta = @"
                DELETE FROM Conta 
                WHERE IdConta = @IdConta 
                  AND IdUsuario = @IdUsuario";
        }
        #endregion

        #region Despesa
        public static class Despesa
        {
            public const string GetDespesas = @"
                SELECT IdDespesa, Valor, Descricao, Data, Recorrente, Frequencia, 
                       IdUsuario, IdConta, IdCategoria, IdMetodoPagamento
                FROM Despesa
                WHERE IdUsuario = @IdUsuario";

            public const string InsertDespesa = @"
                INSERT INTO Despesa (Valor, Descricao, Data, Recorrente, Frequencia, 
                                    IdUsuario, IdConta, IdCategoria, IdMetodoPagamento)
                VALUES (@Valor, @Descricao, @Data, @Recorrente, @Frequencia, 
                        @IdUsuario, @IdConta, @IdCategoria, @IdMetodoPagamento)";

            public const string UpdateDespesa = @"
                UPDATE Despesa
                SET Valor = COALESCE(@Valor, Valor),
                    Descricao = COALESCE(@Descricao, Descricao),
                    Data = COALESCE(@Data, Data),
                    Recorrente = COALESCE(@Recorrente, Recorrente),
                    Frequencia = COALESCE(@Frequencia, Frequencia),
                    IdConta = COALESCE(@IdConta, IdConta),
                    IdCategoria = COALESCE(@IdCategoria, IdCategoria),
                    IdMetodoPagamento = COALESCE(@IdMetodoPagamento, IdMetodoPagamento)
                WHERE IdUsuario = @IdUsuario
                  AND IdDespesa = @IdDespesa";

            public const string DeleteDespesa = @"
                DELETE FROM Despesa
                WHERE IdUsuario = @IdUsuario
                  AND IdDespesa = @IdDespesa";
        }
        #endregion

        #region MetaFinanceira
        public static class MetaFinanceira
        {
            public const string GetMetasFinanceiras = @"
                SELECT IdMetaFinanceira, Descricao, ValorAlvo, ValorAtual, DataInicio, DataLimite, Concluida, IdUsuario
                FROM MetaFinanceira
                WHERE IdUsuario = @IdUsuario";

            public const string InsertMetaFinanceira = @"
                INSERT INTO MetaFinanceira (Descricao, ValorAlvo, ValorAtual, DataInicio, DataLimite, Concluida, IdUsuario)
                VALUES (@Descricao, @ValorAlvo, @ValorAtual, @DataInicio, @DataLimite, @Concluida, @IdUsuario)";

            public const string UpdateMetaFinanceira = @"
                UPDATE MetaFinanceira
                SET Descricao = COALESCE(@Descricao, Descricao),
                    ValorAlvo = COALESCE(@ValorAlvo, ValorAlvo),
                    ValorAtual = COALESCE(@ValorAtual, ValorAtual),
                    DataInicio = COALESCE(@DataInicio, DataInicio),
                    DataLimite = COALESCE(@DataLimite, DataLimite),
                    Concluida = COALESCE(@Concluida, Concluida)
                WHERE IdUsuario = @IdUsuario
                  AND IdMetaFinanceira = @idMetaFinanceira";

            public const string DeleteMetaFinanceira = @"
                DELETE FROM MetaFinanceira
                WHERE IdUsuario = @IdUsuario
                  AND IdMetaFinanceira = @IdMetaFinanceira";
        }
        #endregion

        #region MetodoPagamento
        public static class MetodoPagamento
        {
            public const string GetMetodosPagamentoUsuario = @"
                SELECT IdMetodo, Nome, Descricao, Limite, Tipo, IdUsuario, IdConta
                FROM MetodoPagamento
                WHERE IdUsuario = @IdUsuario";

            public const string InsertMetodoPagamento = @"
                INSERT INTO MetodoPagamento (Nome, Descricao, Limite, Tipo, IdUsuario, IdConta)
                VALUES (@Nome, @Descricao, @Limite, @Tipo, @IdUsuario, @IdConta)";

            public const string UpdateMetodoPagamento = @"
                UPDATE MetodoPagamento
                SET Nome = COALESCE(@Nome, Nome),
                    Descricao = COALESCE(@Descricao, Descricao),
                    Limite = COALESCE(@Limite, Limite),
                    Tipo = COALESCE(@Tipo, Tipo),
                    IdConta = COALESCE(@IdConta, IdConta)
                WHERE IdMetodo = @IdMetodo
                  AND IdUsuario = @IdUsuario";

            public const string DeleteMetodoPagamento = @"
                DELETE FROM MetodoPagamento
                WHERE IdUsuario = @IdUsuario
                  AND IdMetodo = @IdMetodo";
        }
        #endregion

        #region MovimentacaoFinanceira
        public static class MovimentacaoFinanceira
        {
            public const string GetMovimentacoesFinanceiras = @"
                SELECT 
                    IdMovimentacao,
                    TipoMovimentacao,
                    Valor,
                    Data,
                    Descricao,
                    IdConta,
                    IdUsuario
                FROM MovimentacaoFinanceira
                WHERE IdUsuario = @IdUsuario
                /**wherePeriodo**/
                ORDER BY Data DESC";

            public static string ApplyPeriodoFilter(string query, int? periodo)
            {
                return periodo.HasValue
                    ? query.Replace("/**wherePeriodo**/", "AND Data >= DATEADD(MONTH, -@Periodo, GETDATE())")
                    : query.Replace("/**wherePeriodo**/", "");
            }
        }
        #endregion

        #region Receita
        public static class Receita
        {
            public const string GetReceitas = @"
                SELECT IdReceita, Valor, Descricao, Data, Recorrente, Frequencia, 
                       IdUsuario, IdConta, IdCategoria
                FROM Receita
                WHERE IdUsuario = @IdUsuario";

            public const string InsertReceita = @"
                INSERT INTO Receita (Valor, Descricao, Data, Recorrente, Frequencia, 
                                     IdUsuario, IdConta, IdCategoria)
                VALUES (@Valor, @Descricao, @Data, @Recorrente, @Frequencia, 
                         @IdUsuario, @IdConta, @IdCategoria)";

            public const string UpdateReceita = @"
                UPDATE Receita
                SET Valor = COALESCE(@Valor, Valor),
                    Descricao = COALESCE(@Descricao, Descricao),
                    Data = COALESCE(@Data, Data),
                    Recorrente = COALESCE(@Recorrente, Recorrente),
                    Frequencia = COALESCE(@Frequencia, Frequencia),
                    IdConta = COALESCE(@IdConta, IdConta),
                    IdCategoria = COALESCE(@IdCategoria, IdCategoria)
                WHERE IdUsuario = @IdUsuario
                  AND IdReceita = @IdReceita";

            public const string DeleteReceita = @"
                DELETE FROM Receita
                WHERE IdUsuario = @IdUsuario
                  AND IdReceita = @IdReceita";
        }
        #endregion

        #region Usuario
        public static class Usuario
        {
            public const string GetDadosPessoais = @"
                SELECT IdUsuario, Nome, Email, Senha, DataNascimento, Telefone 
                FROM Usuario 
                WHERE IdUsuario = @idUsuario";

            public const string InsertDadosPessoais = @"
                INSERT INTO Usuario (Nome, Email, Senha, DataNascimento, Telefone)
                VALUES (@Nome, @Email, @Senha, @DataNascimento, @Telefone)";

            public const string UpdateDadosPessoais = @"
                UPDATE Usuario 
                SET Nome = COALESCE(@Nome, Nome),
                    Email = COALESCE(@Email, Email),
                    Senha = COALESCE(@Senha, Senha),
                    DataNascimento = COALESCE(@DataNascimento, DataNascimento),
                    Telefone = COALESCE(@Telefone, Telefone)
                WHERE IdUsuario = @idUsuario";

            public const string DeleteConta = @"
                DELETE FROM Usuario WHERE IdUsuario = @idUsuario";
        }
        #endregion
    }
}