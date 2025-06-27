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
            EXEC sp_ExcluirCategoria 
                @IdCategoria = @IdCategoria, 
                @IdUsuario = @IdUsuario";
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

            public const string DeleteConta = @"EXEC sp_ExcluirConta @IdConta = @IdConta, @IdUsuario = @IdUsuario";

        }
        #endregion

        #region Despesa
        public static class Despesa
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
                  AND d.DataDespesa BETWEEN @DataInicio AND @DataFim;";

            public const string GetDespesasPorCategoria = @"
                SELECT 
                    c.Nome AS Categoria,
                    SUM(d.Valor) AS TotalDespesa
                FROM Despesa d
                INNER JOIN Categoria c ON d.IdCategoria = c.IdCategoria
                WHERE d.IdUsuario = @IdUsuario
                  AND d.DataDespesa BETWEEN @DataInicio AND @DataFim
                GROUP BY c.Nome
                ORDER BY TotalDespesa DESC;";

            public const string GetDespesasPorConta = @"
                SELECT 
                    c.NumeroConta,
                    SUM(d.Valor) AS TotalDespesa
                FROM Despesa d
                INNER JOIN Conta c ON d.IdConta = c.IdConta
                WHERE d.IdUsuario = @IdUsuario
                  AND d.DataDespesa BETWEEN @DataInicio AND @DataFim
                GROUP BY c.NumeroConta
                ORDER BY TotalDespesa DESC;";

            public const string GetDespesasPorMetodoPagamento = @"
                SELECT 
                    m.Descricao AS MetodoPagamento,
                    SUM(d.Valor) AS TotalDespesa
                FROM Despesa d
                INNER JOIN MetodoPagamento m ON d.IdMetodoPagamento = m.IdMetodo
                WHERE d.IdUsuario = @IdUsuario
                  AND d.DataDespesa BETWEEN @DataInicio AND @DataFim
                GROUP BY m.Nome
                ORDER BY TotalDespesa DESC;";

            public const string GetTotalDespesasNoPeriodo = @"
                SELECT 
                    SUM(Valor) AS TotalDespesa
                FROM Despesa
                WHERE IdUsuario = @IdUsuario
                  AND DataDespesa BETWEEN @DataInicio AND @DataFim;";

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
                INSERT INTO MetaFinanceira (Descricao, ValorAlvo, ValorAtual, DataInicio, DataLimite, IdUsuario)
                VALUES (@Descricao, @ValorAlvo, @ValorAtual, @DataInicio, @DataLimite, @IdUsuario)";

            public const string UpdateMetaFinanceira = @"
                UPDATE MetaFinanceira
                SET Descricao = COALESCE(@Descricao, Descricao),
                    ValorAlvo = COALESCE(@ValorAlvo, ValorAlvo),
                    ValorAtual = COALESCE(@ValorAtual, ValorAtual),
                    DataInicio = COALESCE(@DataInicio, DataInicio),
                    DataLimite = COALESCE(@DataLimite, DataLimite)                 
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
            EXEC sp_ExcluirMetodoPagamento 
                @IdMetodo = @IdMetodo, 
                @IdUsuario = @IdUsuario";
        }
        #endregion

        #region MovimentacaoFinanceira
        public static class MovimentacaoFinanceira
        {
            public const string GetMovimentacoesPorPeriodo = @"
                SELECT * FROM fn_MovimentacoesPorUsuarioPeriodo(@IdUsuario, @DataInicio, @DataFim);";

            public const string GetSaldoPorConta = @"SELECT * FROM vw_SaldoPorConta WHERE IdUsuario = @IdUsuario;";

            public const string GetSaldoTotalContas = @"SELECT * FROM vw_SaldoTotalUsuario WHERE IdUsuario = 1;";

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
        public static class Receita
        {
            public const string GetReceitasPorId = @"
            SELECT 
                r.IdReceita, 
                r.Valor, 
                r.Descricao, 
                r.DataReceita, 
                r.IdUsuario, 
                c.Nome AS Categoria, 
                ct.NumeroConta AS Conta
            FROM Receita r
            LEFT JOIN Categoria c ON c.IdCategoria = r.IdCategoria
            INNER JOIN Conta ct ON ct.IdConta = r.IdConta
            WHERE r.IdReceita = @IdReceita
                AND r.IdUsuario = @IdUsuario
                AND r.DataReceita BETWEEN @DataInicio AND @DataFim;";

            public const string GetTotalReceitasPeriodo = @"
            SELECT 
                SUM(Valor) AS TotalReceita
            FROM Receita
            WHERE IdUsuario = @IdUsuario
              AND DataReceita BETWEEN @DataInicio AND @DataFim;";

            public const string GetReceitasPorCategoria = @"
            SELECT 
                c.Nome AS Categoria,
                SUM(r.Valor) AS TotalReceita
            FROM Receita r
            INNER JOIN Categoria c ON r.IdCategoria = c.IdCategoria
            WHERE r.IdUsuario = @IdUsuario
              AND r.DataReceita BETWEEN @DataInicio AND @DataFim
            GROUP BY c.Nome
            ORDER BY TotalReceita DESC;";

            public const string GetReceitasPorConta = @"
            SELECT 
                c.NumeroConta,
                SUM(r.Valor) AS TotalReceita
            FROM Receita r
            INNER JOIN Conta c ON r.IdConta = c.IdConta
            WHERE r.IdUsuario = @IdUsuario
              AND r.DataReceita BETWEEN @DataInicio AND @DataFim
            GROUP BY c.NumeroConta
            ORDER BY TotalReceita DESC;";

            public const string InsertReceita = @"
                INSERT INTO Receita (Valor, Descricao, DataReceita, 
                                     IdUsuario, IdConta, IdCategoria)
                VALUES (@Valor, @Descricao, @DataReceita, 
                        @IdUsuario, @IdConta, @IdCategoria)";

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

            public const string DeleteConta =
                @"EXEC sp_ExcluirUsuario @IdUsuario = @idUsuario";
        }
        #endregion
    }
}