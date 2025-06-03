using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IMetodoPagamentoService : INotifiable
    {
        public Task<IEnumerable<MetodoPagamento?>> GetMetodosPagamento(int idUsuario);
        public Task<bool> InsertMetodoPagamento(MetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario);
        public Task<bool> UpdateMetodoPagamento(MetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario);
        public Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento);
    }
}
