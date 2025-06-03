using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface IMovimentacaoFinanceiraUsuarioService
    {
        public Task<MovimentacaoFinanceiraResponseInfra?> GetMovimentacoesFinanceiras(int idUsuario, int periodo);
        public Task<bool> InsertMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario);
        public Task<bool> DeleteMovimentacaoFinanceira(int idMovimentacao, int idUsuario);
    }
}
