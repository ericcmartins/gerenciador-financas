using System.Data;
using Dapper;
using gerenciador.financas.Infra.Vendors.Entities;
using Microsoft.Data.SqlClient;

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

        public async Task<List<MovimentacaoFinanceiraResponseInfra>> GetMovimentacoesFinanceiras(int idUsuario, int? periodo)
        {
            using var connection = _connectionHandler.CreateConnection();

            var instrucaoSql = @" SELECT 
                    IdMovimentacao,
                    TipoMovimentacao,
                    Valor,
                    Data,
                    Descricao,
                    FkContaIdConta,
                    FkUsuarioIdUsuario
                FROM MovimentacaoConta
                WHERE FkUsuarioIdUsuario = @IdUsuario
                /**wherePeriodo**/
                ORDER BY Data DESC
            ";

            // se for passado um período (em meses), filtra pelo campo Data
            if (periodo.HasValue)
            {
                instrucaoSql = instrucaoSql.Replace("/**wherePeriodo**/", "AND Data >= DATEADD(MONTH, -@Periodo, GETDATE())");
            }
            else
            {
                instrucaoSql = instrucaoSql.Replace("/**wherePeriodo**/", "");
            }

            var response = await connection.QueryAsync<MovimentacaoFinanceiraResponseInfra>(
                instrucaoSql, new { IdUsuario = idUsuario, Periodo = periodo }
            );

            var responseList = response.ToList();

            if (!responseList.Any())
                _notificationPool.AddNotification(404, "Não foram encontradas movimentações financeiras para o usuário");

            return responseList;
        }
    }
}