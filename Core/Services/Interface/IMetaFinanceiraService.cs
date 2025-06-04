using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public interface IMetaFinanceiraService : INotifiable
    {
        public Task<List<MetaFinanceira?>> GetMetasFinanceiras(int idUsuario);
        public Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestViewModel metaFinanceiraRequest, int idUsuario);
        public Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestViewModel metaFinanceiraRequest, int idUsuario, int idMetaFinanceira);
        public Task<bool> DeleteMetaFinanceira(int idMetaFinanceira, int idUsuario);
    }
}
