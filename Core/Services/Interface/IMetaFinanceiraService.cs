using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IMetaFinanceiraService : INotifiable
    {
        public Task<MetaFinanceira?> GetMetasFinanceiras(int idUsuario);
        public Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestViewModel metaFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestViewModel metaFinanceiraRequest, int idUsuario);
        public Task<bool> DeleteMetaFinanceira(int idMeta, int idUsuario);
    }
}
