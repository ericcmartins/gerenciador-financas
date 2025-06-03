using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IDespesaService : INotifiable
    {
        public Task<Despesa?> GetDespesas(int idUsuario, int periodo);
        public Task<bool> InsertDespesa(DespesaRequestViewModel despesaRequest, int idUsuario);
        public Task<bool> UpdateConta(DespesaRequestViewModel despesaRequest, int idUsuario);
        public Task<bool> DeleteDespesa(List<int> idDespesa, int idUsuario);
    }
}
