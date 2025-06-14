using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Queries;
using Microsoft.Data.SqlClient;
using static gerenciador.financas.Infra.Vendors.Queries.SqlQueries;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public class MovimentacaoFinanceiraRepository : IMovimentacaoFinanceiraRepository
    {
        private readonly ISqlServerConnectionHandler _connectionHandler;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public MovimentacaoFinanceiraRepository(ISqlServerConnectionHandler connectionHandler,
                                         NotificationPool notificationPool)
        {
            _connectionHandler = connectionHandler;
            _notificationPool = notificationPool;
        }

        public async Task<List<MovimentacaoFinanceiraResponseInfra?>> GetMovimentacoesFinanceiras(int idUsuario, int? periodo)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();

            var parametros = new
            {
                IdUsuario = idUsuario,
                DataInicio = periodo.HasValue ? DateTime.Today.AddDays(-periodo.Value) : DateTime.MinValue,
                DataFim = DateTime.Today,
            };

            var response = await connection.QueryAsync<MovimentacaoFinanceiraResponseInfra>(
                MovimentacaoFinanceira.GetMovimentacoesPorPeriodo, parametros);

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas despesas no período informado para o usuário");

            return responseList;
        }


        public async Task<bool> InsertTransferenciaEntreContas(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario, int idContaOrigem, int idContaDestino)
        {
            using var connection = await _connectionHandler.CreateConnectionAsync();


            var linhasAfetadas = await connection.ExecuteAsync(SqlQueries.MovimentacaoFinanceira.InsertTransferenciaEntreContas, new
            {
                movimentacaoFinanceiraRequest.TipoMovimentacao,
                movimentacaoFinanceiraRequest.Valor,
                movimentacaoFinanceiraRequest.DataMovimentacao,
                movimentacaoFinanceiraRequest.Descricao,
                idUsuario,
                idContaOrigem,
                idContaDestino
            });

            if (linhasAfetadas != 1)
            {
                _notificationPool.AddNotification(500, "Erro ao cadastrar despesa");
                return false;
            }

            return true;
        }


    }
}