using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IMovimentacaoFinanceiraRepository
    {
        public Task<MovimentacaoFinanceiraResponseInfra?> GetMovimentacoesFinanceiras(int idUsuario, int periodo);
        public Task<bool> InsertMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMovimentacaoFinanceira(MovimentacaoFinanceiraRequestInfra movimentacaoFinanceiraRequest, int idUsuario);
        public Task<bool> DeleteMovimentacaoFinanceira(int idMovimentacao, int idUsuario);
    }
}
