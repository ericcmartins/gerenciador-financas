using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;

namespace gerenciador.financas.Infra.Vendors.Queries
{
    public static class SqlQueries
    {
        #region Categoria
        public static class Categorias
        {
            public const string GetCategorias = @"
                SELECT IdCategoria, Nome, Descricao, IdUsuario
                FROM Categoria
                WHERE IdUsuario = @idUsuario";

            public const string InsertCategoria = @"
                INSERT INTO Categoria (Nome, Descricao, Tipo, IdUsuario)
                VALUES (@Nome, @Descricao, @Tipo, @IdUsuario)";

            public const string UpdateCategoria = @"
                UPDATE Categoria
                SET Nome = COALESCE(@Nome, Nome),
                    Descricao = COALESCE(@Descricao, Descricao)
                    Tipo = COALESCE(@Tipo, Tipo)
                WHERE IdCategoria = @IdCategoria
                  AND IdUsuario = @IdUsuario";

            public const string DeleteCategoria = @"
                EXEC sp_ExcluirCategoria 
                    @IdCategoria = @IdCategoria, 
                    @IdUsuario = @IdUsuario";

            public const string InserirCategoriasPadrao = @"
               EXEC sp_InserirCategoriasPadraoParaUsuario @IdUsuario;";
        }
        #endregion

        #region Conta
        public static class Contas
        {
            public const string GetContas = @"
                SELECT c.IdConta, c.NumeroConta, t.Nome as TipoConta, c.Instituicao, c.IdUsuario
                FROM Conta as c
                INNER JOIN TipoConta as t ON t.IdTipoConta = c.IdTipoConta
                WHERE IdUsuario = @idUsuario";

            public const string InsertConta = @"
                INSERT INTO Conta (NumeroConta, IdTipoConta, Instituicao, IdUsuario)
                VALUES (@NumeroConta, @IdTipoConta, @Instituicao, @IdUsuario)";

            public const string UpdateConta = @"
                UPDATE Conta
                SET NumeroConta = COALESCE(@NumeroConta, NumeroConta),
                    IdTipoConta = COALESCE(@IdTipoConta, IdTipoConta),
                    Instituicao = COALESCE(@Instituicao, Instituicao)
                WHERE IdConta = @IdConta
                  AND IdUsuario = @IdUsuario";

            public const string DeleteConta = @"
                EXEC sp_ExcluirConta 
                    @IdConta = @IdConta, 
                    @IdUsuario = @IdUsuario";
        }
        #endregion

        #region Despesa
        public static class Despesas
        {
            public const string GetDespesasPorUsuario = @"
                SELECT 
                    d.IdDespesa, 
                    d.Valor, 
                    d.Descricao, 
                    d.DataDespesa, 
                    d.IdUsuario, 
                    c.Nome AS Categoria, 
                    ct.NumeroConta AS Conta, 
                    mp.Nome AS MetodoPagamento
                FROM Despesa d
                LEFT JOIN Categoria c ON c.IdCategoria = d.IdCategoria
                INNER JOIN Conta ct ON ct.IdConta = d.IdConta
                LEFT JOIN MetodoPagamento mp ON mp.IdMetodo = d.IdMetodoPagamento
                WHERE d.IdUsuario = @IdUsuario
                  AND (
                        ( @DataInicio IS NULL OR d.DataDespesa >= @DataInicio )
                        AND
                        ( @DataFim IS NULL OR d.DataDespesa <= @DataFim )
                      );";

            public const string GetDespesasPorCategoria = @"
                SELECT 
                    c.Nome AS Categoria,
                    SUM(d.Valor) AS TotalDespesa
                FROM Despesa d
                INNER JOIN Categoria c ON d.IdCategoria = c.IdCategoria
                WHERE d.IdUsuario = @IdUsuario
                  AND (
                        (@DataInicio IS NULL OR d.DataDespesa >= @DataInicio)
                        AND (@DataFim IS NULL OR d.DataDespesa <= @DataFim)
                      )
                GROUP BY c.Nome
                ORDER BY TotalDespesa DESC;";

            public const string GetDespesasPorConta = @"
                SELECT 
                    c.NumeroConta,
                    SUM(d.Valor) AS TotalDespesa
                FROM Despesa d
                INNER JOIN Conta c ON d.IdConta = c.IdConta
                WHERE d.IdUsuario = @IdUsuario
                  AND (
                        (@DataInicio IS NULL OR d.DataDespesa >= @DataInicio)
                        AND (@DataFim IS NULL OR d.DataDespesa <= @DataFim)
                      )
                GROUP BY c.NumeroConta
                ORDER BY TotalDespesa DESC;";

            public const string GetDespesasPorMetodoPagamento = @"
                SELECT 
                    m.Nome AS MetodoPagamento,
                    SUM(d.Valor) AS TotalDespesa
                FROM Despesa d
                INNER JOIN MetodoPagamento m ON d.IdMetodoPagamento = m.IdMetodo
                WHERE d.IdUsuario = @IdUsuario
                  AND (
                        (@DataInicio IS NULL OR d.DataDespesa >= @DataInicio)
                        AND (@DataFim IS NULL OR d.DataDespesa <= @DataFim)
                      )
                GROUP BY m.Nome
                ORDER BY TotalDespesa DESC;";

            public const string GetTotalDespesasNoPeriodo = @"
                SELECT 
                    SUM(Valor) AS TotalDespesa
                FROM Despesa
                WHERE IdUsuario = @IdUsuario
                  AND (
                        (@DataInicio IS NULL OR DataDespesa >= @DataInicio)
                        AND (@DataFim IS NULL OR DataDespesa <= @DataFim)
                      );";

            public const string InsertDespesa = @"
                INSERT INTO Despesa (Valor, Descricao, DataDespesa, 
                                     IdUsuario, IdConta, IdCategoria, IdMetodoPagamento)
                VALUES (@Valor, @Descricao, @DataDespesa, 
                        @IdUsuario, @IdConta, @IdCategoria, @IdMetodoPagamento)";

            public const string UpdateDespesa = @"
                UPDATE Despesa
                SET Valor = COALESCE(@Valor, Valor),
                    Descricao = COALESCE(@Descricao, Descricao),
                    DataDespesa = COALESCE(@DataDespesa, DataDespesa),
                    IdConta = COALESCE(@IdConta, IdConta),
                    IdCategoria = COALESCE(@IdCategoria, IdCategoria),
                    IdMetodoPagamento = COALESCE(@IdMetodoPagamento, IdMetodoPagamento)
                WHERE IdUsuario = @IdUsuario
                  AND IdDespesa = @IdDespesa";

            public const string DeleteDespesa = @"
                EXEC sp_ExcluirDespesa 
                    @IdDespesa = @IdDespesa, 
                    @IdUsuario = @IdUsuario";
        }
        #endregion

        #region MetaFinanceira
        public static class MetasFinanceiras
        {
            public const string GetMetasFinanceiras = @"
                SELECT IdMetaFinanceira, Nome, Descricao, ValorAlvo, ValorAtual, DataInicio, DataLimite, Concluida, IdUsuario
                FROM MetaFinanceira
                WHERE IdUsuario = @IdUsuario";

            public const string InsertMetaFinanceira = @"
                INSERT INTO MetaFinanceira (Nome, Descricao, ValorAlvo, ValorAtual, DataInicio, DataLimite, Concluida, IdUsuario)
                VALUES (@Nome, @Descricao, @ValorAlvo, @ValorAtual, @DataInicio, @DataLimite, @Concluida, @IdUsuario)";

            public const string UpdateMetaFinanceira = @"
                UPDATE MetaFinanceira
                SET Nome = COALESCE(@Nome, Nome),
                    Descricao = COALESCE(@Descricao, Descricao),
                    ValorAlvo = COALESCE(@ValorAlvo, ValorAlvo),
                    ValorAtual = COALESCE(@ValorAtual, ValorAtual),
                    DataInicio = COALESCE(@DataInicio, DataInicio),
                    DataLimite = COALESCE(@DataLimite, DataLimite),
                    Concluida = COALESCE(@Concluida, Concluida)
                WHERE IdUsuario = @IdUsuario
                  AND IdMetaFinanceira = @IdMetaFinanceira";

            public const string DeleteMetaFinanceira = @"
                DELETE FROM MetaFinanceira
                WHERE IdUsuario = @IdUsuario
                  AND IdMetaFinanceira = @IdMetaFinanceira";
        }
        #endregion

        #region MetodoPagamento
        public static class MetodosPagamento
        {
            public const string GetMetodosPagamentoUsuario = @"SELECT 
                mp.IdMetodo,
                mp.Nome,
                t.Nome as TipoMetodoPagamento,
                mp.IdTipoMetodo,
                mp.Limite,
                mp.IdUsuario,
                mp.IdConta,
                c.NumeroConta
            FROM MetodoPagamento mp
            LEFT JOIN Conta c ON c.IdConta = mp.IdConta
            INNER JOIN TipoMetodoPagamento t ON t.IdTipoMetodo = mp.IdTipoMetodo
            WHERE mp.IdUsuario = @IdUsuario;";

            public const string InsertMetodoPagamento = @"
                INSERT INTO MetodoPagamento (Nome, IdTipoMetodo, Limite, IdUsuario, IdConta)
                VALUES (@Nome, @IdTipoMetodo, @Limite, @IdUsuario, @IdConta)";

            public const string UpdateMetodoPagamento = @"
                UPDATE MetodoPagamento
                SET Nome = COALESCE(@Nome, Nome),
                    IdTipoMetodo = COALESCE(@IdTipoMetodo, IdTipoMetodo),
                    Limite = COALESCE(@Limite, Limite), 
                    IdConta = COALESCE(@IdConta, IdConta)
                WHERE IdMetodo = @IdMetodo
                  AND IdUsuario = @IdUsuario";

            public const string DeleteMetodoPagamento = @"
                EXEC sp_ExcluirMetodoPagamento 
                    @IdMetodo = @IdMetodo, 
                    @IdUsuario = @IdUsuario";
        }
        #endregion

        #region MovimentacaoFinanceira
        public static class MovimentacoesFinanceiras
        {
            public const string GetMovimentacoesPorPeriodo = @"
                SELECT * FROM fn_MovimentacoesPorUsuarioPeriodo(@IdUsuario, @DataInicio, @DataFim, @TipoMovimentacao);";

            public const string GetSaldoPorConta = @"
                SELECT * FROM vw_SaldoPorConta WHERE IdUsuario = @idUsuario;";

            public const string GetSaldoTotalContas = @"
                SELECT * FROM vw_SaldoTotalUsuario WHERE IdUsuario = @idUsuario;";

            public const string InsertTransferenciaEntreContas = @"
                INSERT INTO MovimentacaoFinanceira (TipoMovimentacao, Valor, DataMovimentacao, Descricao, IdContaOrigem, IdContaDestino, IdUsuario, IdReceita, IdDespesa) 
                VALUES (@TipoMovimentacao, @Valor, @DataMovimentacao, @Descricao, @IdContaOrigem, @IdContaDestino, @IdUsuario, NULL, NULL);";

            public const string UpdateMovimentacaoFinanceira = @"
                UPDATE MovimentacaoFinanceira
                SET
                    TipoMovimentacao = COALESCE(@TipoMovimentacao, TipoMovimentacao),
                    Valor = COALESCE(@Valor, Valor),
                    DataMovimentacao = COALESCE(@DataMovimentacao, DataMovimentacao),
                    Descricao = COALESCE(@Descricao, Descricao),
                    IdContaOrigem = COALESCE(@IdContaOrigem, IdContaOrigem),
                    IdContaDestino = COALESCE(@IdContaDestino, IdContaDestino)
                WHERE IdMovimentacao = @IdMovimentacao;";

            public const string DeleteMovimentacaoFinanceira = @"
                DELETE FROM MovimentacaoFinanceira
                WHERE IdMovimentacao = @IdMovimentacao;";
        }
        #endregion

        #region Receita
        public static class Receitas
        {
            //public const string GetReceitasPorIdUsuario = @"
            //    SELECT 
            //        r.IdReceita, 
            //        r.Valor, 
            //        r.Descricao, 
            //        r.DataReceita, 
            //        r.IdUsuario, 
            //        c.Nome AS Categoria, 
            //        ct.NumeroConta AS Conta
            //    FROM Receita r
            //    LEFT JOIN Categoria c ON c.IdCategoria = r.IdCategoria
            //    INNER JOIN Conta ct ON ct.IdConta = r.IdConta
            //    WHERE r.IdUsuario = @IdUsuario
            //      AND (
            //            (@DataInicio IS NULL OR r.DataReceita >= @DataInicio)
            //            AND (@DataFim IS NULL OR r.DataReceita <= @DataFim)
            //          );";

            public const string GetReceitasPorIdUsuario = @"
                 SELECT 
                        r.IdReceita, 
                        r.Valor, 
                        r.Descricao, 
                        r.DataReceita, 
                        r.IdUsuario, 
                        c.Nome AS Categoria, 
                        ct.Instituicao,
                        tc.Nome As TipoConta
                    FROM Receita r
                    LEFT JOIN Categoria c ON c.IdCategoria = r.IdCategoria
                    LEFT JOIN Conta ct ON ct.IdConta = r.IdConta
                    LEFT JOIN TipoConta tc ON tc.IdTipoConta = ct.IdTipoConta
                WHERE r.IdUsuario = @IdUsuario
                  AND r.DataReceita >= CAST(DATEADD(DAY, -@QtdDias, GETDATE()) AS DATE)
                  AND r.DataReceita < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))";

            public const string GetTotalReceitasPeriodo = @"
                SELECT 
                    SUM(Valor) AS TotalReceita
                FROM Receita
                WHERE IdUsuario = @IdUsuario
                  AND (
                        (@DataInicio IS NULL OR DataReceita >= @DataInicio)
                        AND (@DataFim IS NULL OR DataReceita <= @DataFim)
                      );";

            public const string GetReceitasPorCategoria = @"
                SELECT 
                    ISNULL(c.Nome, 'Sem categoria') AS Categoria,
                    SUM(r.Valor) AS TotalReceita
                FROM Receita r
                LEFT JOIN Categoria c ON r.IdCategoria = c.IdCategoria
                WHERE r.IdUsuario = @IdUsuario
                  AND (
                        (@DataInicio IS NULL OR r.DataReceita >= @DataInicio)
                        AND (@DataFim IS NULL OR r.DataReceita <= @DataFim)
                      )
                GROUP BY c.Nome
                ORDER BY TotalReceita DESC;";


            public const string GetReceitasPorConta = @"
                SELECT 
                    c.NumeroConta,
                    SUM(r.Valor) AS TotalReceita
                FROM Receita r
                INNER JOIN Conta c ON r.IdConta = c.IdConta
                WHERE r.IdUsuario = @IdUsuario
                  AND (
                        (@DataInicio IS NULL OR r.DataReceita >= @DataInicio)
                        AND (@DataFim IS NULL OR r.DataReceita <= @DataFim)
                      )
                GROUP BY c.NumeroConta
                ORDER BY TotalReceita DESC;";

            public const string InsertReceita = @"
                INSERT INTO Receita (Valor, Descricao, DataReceita, IdUsuario, IdConta, IdCategoria)
                VALUES (@Valor, @Descricao, @DataReceita, @IdUsuario, @IdConta, @IdCategoria)";

            public const string UpdateReceita = @"
                UPDATE Receita
                SET Valor = COALESCE(@Valor, Valor),
                    Descricao = COALESCE(@Descricao, Descricao),
                    DataReceita = COALESCE(@DataReceita, DataReceita),
                    IdConta = COALESCE(@IdConta, IdConta),
                    IdCategoria = COALESCE(@IdCategoria, IdCategoria)
                WHERE IdUsuario = @IdUsuario
                  AND IdReceita = @IdReceita";

            public const string DeleteReceita = @"
                EXEC sp_ExcluirReceita 
                    @IdReceita = @IdReceita, 
                    @IdUsuario = @IdUsuario";
        }
        #endregion

        #region Usuario
        public static class Usuarios
        {
            public const string GetDadosPessoais = @"
                SELECT IdUsuario, Nome, Email, SenhaHash, RoleUsuario, DataNascimento, Telefone 
                FROM Usuario 
                WHERE IdUsuario = @idUsuario";

            public const string InsertDadosPessoais = @"
                DECLARE @NovoIdUsuario INT;
                EXEC sp_CadastrarUsuarioCompleto
                    @Nome = @Nome,
                    @Email = @Email,
                    @SenhaHash = @SenhaHash,
                    @DataNascimento = @DataNascimento,
                    @Telefone = @Telefone,
                    @RoleUsuario = @RoleUsuario,
                    @NovoIdUsuario = @NovoIdUsuario OUTPUT;";

            public const string UpdateDadosPessoais = @"
                UPDATE Usuario 
                SET Nome = COALESCE(@Nome, Nome),
                    Email = COALESCE(@Email, Email),
                    SenhaHash = COALESCE(@SenhaHash, SenhaHash),
                    DataNascimento = COALESCE(@DataNascimento, DataNascimento),
                    Telefone = COALESCE(@Telefone, Telefone)
                WHERE IdUsuario = @idUsuario";

            public const string DeleteUsuario = @"EXEC sp_ExcluirUsuario @IdUsuario = @idUsuario";

            public const string Login = @"
                SELECT IdUsuario, Email, SenhaHash, RoleUsuario as Role
                FROM Usuario
                WHERE Email = @Email";

            public const string GetDadosPessoaisPorEmail = @"
                SELECT IdUsuario, Nome, Email, SenhaHash, DataNascimento, Telefone 
                FROM Usuario 
                WHERE Email = @Email";
        }
        #endregion
    }
}