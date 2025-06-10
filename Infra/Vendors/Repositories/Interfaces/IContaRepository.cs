using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IContaRepository : INotifiable
    {
        public Task<List<ContaResponseInfra?>> GetContas(int idUsuario);
        public Task<bool> InsertConta(ContaRequestInfra contaRequest, int idUsuario);
        public Task<bool> UpdateConta(ContaRequestInfra contaRequest, int idUsuario, int idConta);
        public Task<bool> DeleteConta(int idConta, int idUsuario);
    }
}
