using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IReceitaRepository
    {
        public Task<ReceitaResponseInfra?> GetReceita(int idUsuario);
        public Task<bool> InsertReceita(ReceitaRequestInfra receitaRequest, int idUsuario);
        public Task<bool> UpdateReceita(ReceitaRequestInfra receitaRequest, int idUsuario);
        public Task<bool> DeleteReceita(int idUsuario);
    }
}
