using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;
using static gerenciador.financas.Infra.Vendors.Queries.SqlQueries;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public TransacaoRepository(ISqlServerConnectionHandler connectionHandler,
                                              NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<MovimentacaoFinanceiraResponseInfra?>> GetMovimentacoesFinanceiras(int idUsuario, int periodo, string? tipoMovimentacao)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<MovimentacaoFinanceiraResponseInfra>(Transacoes.GetMovimentacoesPorPeriodo,
                new
                {
                    IdUsuario = idUsuario,
                    Periodo = periodo,
                    TipoMovimentacao = tipoMovimentacao
                });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas transferências no período informado para o usuário");

            return responseList;
        }

        public async Task<List<SaldoPorContaResponseInfra?>> GetSaldoPorConta(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<SaldoPorContaResponseInfra>(
                Transacoes.GetSaldoPorConta, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Erro ao obter saldo por conta");

            return responseList;
        }

        public async Task<List<SaldoTotalUsuarioResponseInfra?>> GetSaldoTotalContas(int idUsuario)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var response = await connection.QueryAsync<SaldoTotalUsuarioResponseInfra>(
                Transacoes.GetSaldoTotalContas, new { idUsuario });

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Erro ao obter saldo total em contas");

            return responseList;
        }

        public async Task<bool> InsertTransferenciaEntreContas(CadastrarTransacaoRequestInfra transacaoRequest, int idUsuario, int idContaOrigem, int idContaDestino)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Transacoes.InsertTransferenciaEntreContas, new
            {
                transacaoRequest.TipoMovimentacao,
                transacaoRequest.Valor,
                transacaoRequest.DataMovimentacao,
                transacaoRequest.Descricao,
                IdUsuario = idUsuario,
                IdContaOrigem = idContaOrigem,
                IdContaDestino = idContaDestino
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao registrar transação na base");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateMovimentacaoFinanceira(AtualizarTransacaoRequestInfra transacaoRequest, int idUsuario, int idContaOrigem, int idContaDestino, int idMovimentacaoFinanceira)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Transacoes.UpdateMovimentacaoFinanceira, new
            {
                transacaoRequest.Valor,
                transacaoRequest.DataMovimentacao,
                transacaoRequest.Descricao,
                IdUsuario = idUsuario,
                IdContaOrigem = idContaOrigem,
                IdContaDestino = idContaDestino,
                IdMovimentacao = idMovimentacaoFinanceira
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao atualizar transação na base");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteMovimentacaoFinanceira(int idUsuario, int idMovimentacaoFinanceira)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.Transacoes.DeleteMovimentacaoFinanceira, new
            {
                IdMovimentacao = idMovimentacaoFinanceira
            });

            if (linhasAfetadas == 0)
            {
                _notificationPool.AddNotification(500, "Erro ao deletar transferência");
                return false;
            }

            return true;
        }
    }

}