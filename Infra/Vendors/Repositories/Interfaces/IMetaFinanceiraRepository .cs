using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IMetaFinanceiraRepository : INotifiable
    {
        public Task<List<MetaFinanceiraResponseInfra?>> GetMetasFinanceiras(int idUsuario);
        public Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario, int idMetaFinanceira);
        public Task<bool> DeleteMetaFinanceira(int idMetaFinanceira, int idUsuario);
    }
}
