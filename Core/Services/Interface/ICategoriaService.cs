using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface ICategoriaService
    {
        public Task<IEnumerable<Categoria>?> GetCategorias(int idUsuario);
        public Task<bool> InsertCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario);
        public Task<bool> UpdateCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario);
        public Task<bool> DeleteCategoria(string nomeCategoria, int idUsuario);
    }
}
