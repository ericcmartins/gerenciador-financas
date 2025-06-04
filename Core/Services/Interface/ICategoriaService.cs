using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface ICategoriaService : INotifiable
    {
        public Task<List<Categoria>?> GetCategorias(int idUsuario);
        public Task<bool> InsertCategoria(CategoriaRequestViewModel categoriaRequest, int idUsuario);
        public Task<bool> UpdateCategoria(CategoriaRequestViewModel categoriaRequest, int idUsuario);
        public Task<bool> DeleteCategoria(string nomeCategoria, int idUsuario);
    }
}
