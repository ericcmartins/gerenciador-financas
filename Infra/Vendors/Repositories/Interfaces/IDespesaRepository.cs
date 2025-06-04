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
        public Task<List<DespesaResponseInfra?>> GetDespesas(int idUsuario);
        public Task<bool> InsertDespesa(DespesaRequestInfra receitaRequest, int idUsuario, int idConta, int idCategoria, int idMetodoPagamento);
        public Task<bool> UpdateDespesa(DespesaRequestInfra receitaRequest, int idUsuario, int idDespesa, int idCategoria, int idConta, int idMetodoPagamento);
        public Task<bool> DeleteDespesa(int idUsuario, int idDespesa);
    }
}
