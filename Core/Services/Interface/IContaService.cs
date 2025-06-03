using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface IContaService
    {
        public Task<IEnumerable<Conta?>> GetConta(int idUsuario);
        public Task<bool> InsertConta(ContaRequestInfra contaRequest, int idUsuario);
        public Task<bool> UpdateConta(ContaRequestInfra contaRequest, int idUsuario);
        public Task<bool> DeleteConta(string numeroConta, int idUsuario);
    }
}
