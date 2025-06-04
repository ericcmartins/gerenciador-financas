using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public interface IMetodoPagamentoService : INotifiable
    {
        public Task<List<MetodoPagamento?>> GetMetodosPagamentoUsuario(int idUsuario);
        public Task<bool> InsertMetodoPagamento(MetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario, int idConta);
        public Task<bool> UpdateMetodoPagamento(MetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario, int idConta, int idMetodoPagamento);
        public Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento);
    }
}
