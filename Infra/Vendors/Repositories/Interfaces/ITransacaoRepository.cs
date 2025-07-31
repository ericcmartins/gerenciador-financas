using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface ITransacaoRepository : INotifiable
    {
        public Task<List<MovimentacaoFinanceiraResponseInfra?>> GetMovimentacoesFinanceiras(int idUsuario, int periodo, string? tipoMovimentacao);
        public Task<List<SaldoPorContaResponseInfra?>> GetSaldoPorConta(int idUsuario);
        public Task<List<SaldoTotalUsuarioResponseInfra?>> GetSaldoTotalContas(int idUsuario);
        public Task<bool> InsertTransferenciaEntreContas(CadastrarTransacaoRequestInfra transacaoRequest, int idUsuario, int idContaOrigem, int contaDestino);
        public  Task<bool> UpdateMovimentacaoFinanceira(AtualizarTransacaoRequestInfra transacaoRequest, int idUsuario, int idContaOrigem, int idContaDestino, int idMovimentacaoFinanceira);
        public Task<bool> DeleteMovimentacaoFinanceira(int idUsuario, int idMovimentacaoFinanceira);
    }
}
