using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface IListaCompraService
    {
        public Task<ListaCompra?> GetListaCompra(string cpf);
        public Task<bool> InsertListaCompra(ListaCompraRequestInfra listaCompraRequest);
        public Task<bool> UpdateListaCompra(ListaCompraRequestInfra listaCompraRequest, string cpf);
        public Task<bool> DeleteListaCompra(string cpf);
        public Task<ItemListaCompra?> GetItemListaCompra(string cpf);
        public Task<bool> InsertItemListaCompra(ItemListaCompraRequestInfra itemListaRequest);
        public Task<bool> UpdateItemListaCompra(ItemListaCompraRequestInfra itemListaRequest, string cpf);
        public Task<bool> DeleteItemListaCompraa(string cpf);
    }
}
