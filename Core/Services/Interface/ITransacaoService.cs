using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;
namespace gerenciador.financas.Application.Services
{
    public interface ITransacaoService : INotifiable
    {
        public Task<List<MovimentacaoFinanceira?>> GetMovimentacoesFinanceiras(int idUsuario, int periodo, string? tipoMovimentacao);
        public Task<List<SaldoContas?>> GetSaldoPorConta(int idUsuario);
        public Task<List<SaldoTotalContas?>> GetSaldoTotalContas(int idUsuario);
        public Task<bool> InsertTransferenciaEntreContas(CadastrarTransacaoRequestViewModel transacaoRequest, int idUsuario, int idContaOrigem, int idContaDestino);
        public Task<bool> UpdateMovimentacaoFinanceira(AtualizarTransacaoRequestViewModel transacaoRequest, int idUsuario, int idContaOrigem, int idContaDestino, int idMovimentacaoFinanceira);
        public Task<bool> DeleteMovimentacaoFinanceira(int idUsuario, int idMovimentacaoFinanceira);
    }
}
