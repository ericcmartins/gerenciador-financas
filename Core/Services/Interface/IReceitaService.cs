using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public interface IReceitaService : INotifiable
    {
        public Task<List<Receita?>> GetReceitasPorUsuario(int idUsuario, int periodo);
        public Task<List<ReceitaCategoria?>> GetReceitasPorCategoria(int idUsuario, int? periodo);
        public Task<List<ReceitaConta?>> GetReceitasPorConta(int idUsuario, int? periodo);
        public Task<Decimal?> GetReceitasTotalPorPeriodo(int idUsuario, int? periodo); 
        public Task<bool> InsertReceita(CadastrarReceitaRequestViewModel receitaRequest, int idUsuario, int idCategoria, int idConta);
        public Task<bool> UpdateReceita(AtualizarReceitaRequestViewModel receitaRequest, int idUsuario, int idReceita, int idCategoria, int idConta);
        public Task<bool> DeleteReceita(int idUsuario, int idReceita);
    }
}
