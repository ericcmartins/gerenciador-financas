using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IMovimentacaoFinanceiraRepository : INotifiable
    {
        public Task<List<MovimentacaoFinanceiraResponseInfra?>> GetMovimentacoesFinanceiras(int idUsuario, int? periodo);
        public Task<bool> InsertTransferenciaEntreContas(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario, int idContaOrigem, int contaDestino);
        public  Task<bool> UpdateMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario, int idContaOrigem, int idContaDestino, int idMovimentacaoFinanceira);
        public Task<bool> DeleteMovimentacaoFinanceira(int idUsuario, int idMovimentacaoFinanceira);
    }
}
