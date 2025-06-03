using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IReceitaService : INotifiable
    {
        public Task<Receita?> GetReceita(int idUsuario);
        public Task<bool> InsertReceita(ReceitaRequestViewModel receitaRequest, int idUsuario);
        public Task<bool> UpdateReceita(ReceitaRequestViewModel receitaRequest, int idUsuario);
        public Task<bool> DeleteReceita(int idUsuario);
    }
}
