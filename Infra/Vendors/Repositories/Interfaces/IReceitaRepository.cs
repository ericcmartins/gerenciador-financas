using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IReceitaRepository : INotifiable
    {
        public Task<List<ReceitaResponseInfra?>> GetReceitasPorUsuario(int idUsuario, int periodo);
        public Task<List<ReceitaPorCategoriaResponseInfra?>> GetReceitasPorCategoria(int idUsuario, int? periodo);
        public Task<List<ReceitaPorContaResponseInfra?>> GetReceitasPorConta(int idUsuario, int? periodo);
        public Task<Decimal> GetReceitasTotalPorPeriodo(int idUsuario, int? periodo);
        public Task<bool> InsertReceita(CadastrarReceitaRequestInfra receitaRequest, int idUsuario, int idCategoria, int idConta);
        public Task<bool> UpdateReceita(AtualizarReceitaRequestInfra receitaRequest, int idUsuario, int idReceita, int idCategoria, int idConta);
        public Task<bool> DeleteReceita(int idUsuario, int idReceita);
    }
}
