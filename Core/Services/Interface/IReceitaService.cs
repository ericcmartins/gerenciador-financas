using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface IReceitaService
    {
        public Task<Receita?> GetReceita(int idUsuario);
        public Task<bool> InsertReceita(ReceitaRequestInfra receitaRequest, int idUsuario);
        public Task<bool> UpdateReceita(ReceitaRequestInfra receitaRequest, int idUsuario);
        public Task<bool> DeleteReceita(int idUsuario);
    }
}
