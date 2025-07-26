using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface ICategoriaRepository : INotifiable
    {
        public Task<List<CategoriaResponseInfra>?> GetCategoriasPorUsuario(int idUsuario);
        public Task<bool> InsertCategoria(CadastrarCategoriaRequestInfra categoriaRequest, int idUsuario);
        public Task<bool> InsertCategoriasPadraoParaUsuario(int idUsuario);
        public Task<bool> UpdateCategoria(AtualizarCategoriaRequestInfra categoriaRequest, int idCategoria, int idUsuario);
        public Task<bool> DeleteCategoria(int idCategoria, int idUsuario);
    }
}
