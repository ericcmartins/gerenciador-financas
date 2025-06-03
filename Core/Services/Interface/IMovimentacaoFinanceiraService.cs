using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public interface IMovimentacaoFinanceiraService : INotifiable
    {
        public Task<MovimentacaoFinanceiraResponseInfra?> GetMovimentacoesFinanceiras(int idUsuario, int periodo);
        public Task<bool> InsertMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario);
        public Task<bool> DeleteMovimentacaoFinanceira(int idMovimentacao, int idUsuario);
    }
}
