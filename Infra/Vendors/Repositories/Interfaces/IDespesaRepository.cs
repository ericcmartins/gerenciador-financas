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
        public Task<DespesaResponseInfra?> GetDespesas(int idUsuario, int periodo);
        public Task<bool> InsertDespesa(DespesaRequestInfra despesaRequest, int idUsuario);
        public Task<bool> UpdateConta(DespesaRequestInfra despesaRequest, int idUsuario);
        public Task<bool> DeleteDespesa(List<int> idDespesa, int idUsuario);
    }
}
