using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface IDespesaService
    {
        public Task<Despesa?> GetDespesas(int idUsuario, int periodo);
        public Task<bool> InsertDespesa(DespesaRequestInfra despesaRequest, int idUsuario);
        public Task<bool> UpdateConta(DespesaRequestInfra despesaRequest, int idUsuario);
        public Task<bool> DeleteDespesa(List<int> idDespesa, int idUsuario);
    }
}
