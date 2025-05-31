using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IMetaFinanceiraRepository
    {
        public Task<MetaFinanceiraResponseInfra?> GetMetasFinanceiras(int idUsuario);
        public Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario);
        public Task<bool> DeleteMetaFinanceira(int idMeta, int idUsuario);
    }
}
