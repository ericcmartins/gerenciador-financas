using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{ 
    public interface ICategoriaService : INotifiable
    {
        public Task<List<Categoria>?> GetCategoriasPorUsuario(int idUsuario);
        public Task<bool> InsertCategoria(CadastrarCategoriaRequestViewModel categoriaRequest, int idUsuario);
        public Task<bool> UpdateCategoria(AtualizarCategoriaRequestViewModel categoriaRequest, int idCategoria, int idUsuario);
        public Task<bool> DeleteCategoria(int idCategoria, int idUsuario);
    }
}
