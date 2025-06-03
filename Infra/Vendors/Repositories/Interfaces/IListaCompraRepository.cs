using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IListaCompraRepository : INotifiable
    {
        public Task<ListaCompraResponseInfra?> GetListaCompra(string cpf);
        public Task<bool> InsertListaCompra(ListaCompraRequestInfra listaCompraRequest);
        public Task<bool> UpdateListaCompra(ListaCompraRequestInfra listaCompraRequest, string cpf);
        public Task<bool> DeleteListaCompra(string cpf);
        public Task<ItemListaCompraResponseInfra?> GetItemListaCompra(string cpf);
        public Task<bool> InsertItemListaCompra(ItemListaCompraRequestInfra itemListaRequest);
        public Task<bool> UpdateItemListaCompra(ItemListaCompraRequestInfra itemListaRequest, string cpf);
        public Task<bool> DeleteItemListaCompraa(string cpf);
    }
}
