using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IDespesaRepository : INotifiable
    {
        public Task<List<DespesaResponseInfra?>> GetDespesas(int idUsuario, int? periodo);
        public Task<List<DespesaPorCategoriaResponseInfra?>> GetDespesasPorCategoria(int idUsuario, int? periodo);
        public Task<List<DespesaPorContaResponseInfra?>> GetDespesasPorConta(int idUsuario, int? periodo);
        public Task<List<DespesaPorMetodoPagamentoResponseInfra?>> GetDespesasPorMetodoPagamento(int idUsuario, int? periodo);
        public Task<Decimal> GetTotalDespesasPeriodo(int idUsuario, int? periodo);
        public Task<bool> InsertDespesa(DespesaRequestInfra despesaRequest, int idUsuario, int idCategoria, int idConta, int idMetodoPagamento);
        public Task<bool> UpdateDespesa(DespesaRequestInfra despesaRequest, int idUsuario, int idDespesa, int idCategoria, int idConta, int idMetodoPagamento);
        public Task<bool> DeleteDespesa(int idUsuario, int idDespesa);
    }
}
