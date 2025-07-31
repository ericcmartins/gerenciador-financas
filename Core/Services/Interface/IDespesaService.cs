using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;
using System.Numerics;

namespace gerenciador.financas.Application.Services
{
    public interface IDespesaService : INotifiable
    {
        public Task<List<Despesa?>> GetDespesasPorUsuario(int idUsuario, int periodo);
        public Task<List<DespesaCategoria?>> GetDespesasPorCategoria(int idUsuario, int periodo);
        public Task<List<DespesaConta?>> GetDespesasPorConta(int idUsuario, int periodo);
        public Task<List<DespesaMetodoPagamento?>> GetDespesasPorMetodoPagamento(int idUsuario, int periodo);
        public Task<Decimal?> GetTotalDespesasPeriodo(int idUsuario, int periodo);
        public Task<bool> InsertDespesa(CadastrarDespesaRequestViewModel despesaRequest, int idUsuario, int? idCategoria, int idMetodoPagamento);
        public Task<bool> UpdateDespesa(AtualizarDespesaRequestViewModel despesaRequest, int idUsuario, int idDespesa, int? idCategoria, int idMetodoPagamento);
        public Task<bool> DeleteDespesa(int idUsuario, int idDespesa);
    }
}
