using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IContaService : INotifiable
    {
        public Task<List<Conta?>> GetContas(int idUsuario);
        public Task<bool> InsertConta(ContaRequestViewModel contaRequest, int idUsuario);
        public Task<bool> UpdateConta(ContaRequestViewModel contaRequest, int idUsuario, int idConta);
        public Task<bool> DeleteConta(int idConta, int idUsuario);
    }
}
