using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;
using static gerenciador.financas.Infra.Vendors.Queries.SqlQueries;

namespace gerenciador.financas.Application.Services
{
    public interface IMovimentacaoFinanceiraService : INotifiable
    {
        public Task<List<MovimentacaoFinanceira?>> GetMovimentacoesFinanceiras(int idUsuario, int? periodo, string? tipoMovimentacao);
        //public Task<bool> InsertMovimentacaoFinanceira(transferenciaRequest, idContaOrigem, idContaDestino, idUsuario);
        ////public Task<bool> UpdateMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario, int idConta, int idMovimentacaoFinanceira);
        ////public Task<bool> DeleteMovimentacaoFinanceira(int idMovimentacao, int idUsuario);
    }
}
