using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface IMetaFinanceiraService
    {
        public Task<MetaFinanceira?> GetMetasFinanceiras(int idUsuario);
        public Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestInfra metaFinanceiraRequest, int idUsuario);
        public Task<bool> DeleteMetaFinanceira(int idMeta, int idUsuario);;
    }
}
