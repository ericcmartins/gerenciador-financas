using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public interface IListaCompraService : INotifiable
    {
        public Task<ListaCompra?> GetListaCompra(string cpf);
        public Task<bool> InsertListaCompra(ListaCompraRequestViewModel listaCompraRequest);
        public Task<bool> UpdateListaCompra(ListaCompraRequestViewModel listaCompraRequest, string cpf);
        public Task<bool> DeleteListaCompra(string cpf);
        public Task<ItemListaCompra?> GetItemListaCompra(string cpf);
        public Task<bool> InsertItemListaCompra(ListaCompraRequestViewModel itemListaRequest);
        public Task<bool> UpdateItemListaCompra(ListaCompraRequestViewModel itemListaRequest, string cpf);
        public Task<bool> DeleteItemListaCompraa(string cpf);
    }
}
