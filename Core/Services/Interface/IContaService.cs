using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public interface IContaService : INotifiable
    {
        public Task<List<Conta?>> GetContasPorUsuario(int idUsuario);
        public Task<bool> InsertConta(CadastrarContaRequestViewModel contaRequest, int idUsuario);
        public Task<bool> UpdateConta(AtualizarContaRequestViewModel contaRequest, int idUsuario, int idConta);
        public Task<bool> DeleteConta(int idConta, int idUsuario);
    }
}
