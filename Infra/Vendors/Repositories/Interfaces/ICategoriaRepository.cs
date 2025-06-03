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
        public Task<IEnumerable<CategoriaResponseInfra>?> GetCategorias(int idUsuario);
        public Task<bool> InsertCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario);
        public Task<bool> UpdateCategoria(CategoriaRequestInfra categoriaRequest, int idUsuario);
        public Task<bool> DeleteCategoria(string nomeCategoria, int idUsuario);
    }
}
