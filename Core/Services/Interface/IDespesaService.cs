using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IDespesaService : INotifiable
    {
        public Task<List<Despesa?>> GetDespesas(int idUsuario);
        public Task<bool> InsertDespesa(DespesaRequestViewModel despesaRequest, int idUsuario, int idConta, int idCategoria, int idMetodoPagamento);
        public Task<bool> UpdateDespesa(DespesaRequestViewModel despesaRequest, int idUsuario, int idDespesa, int idCategoria, int idConta, int idMetodoPagamento);
        public Task<bool> DeleteDespesa(int idUsuario, int idDespesa);
    }
}
