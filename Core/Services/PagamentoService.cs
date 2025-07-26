using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public PagamentoService(IPagamentoRepository PagamentoRepository, 
                              NotificationPool notificationPool)
        {
            _pagamentoRepository = PagamentoRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<MetodoPagamento?>> GetMetodosPagamentoUsuario(int idUsuario)
        {
            var responseInfra = await _pagamentoRepository.GetMetodosPagamentoUsuario(idUsuario);
            if (HasNotifications)
                return null;

            var metodosPagamento = responseInfra
                .Select(m => m.ToService())
                .ToList();

            return metodosPagamento;
        }

        public async Task<bool> InsertMetodoPagamento(CadastrarMetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario, int idConta)
        {
            var resultado = await _pagamentoRepository.InsertMetodoPagamento(metodoPagamentoRequest.ToInfra(), idUsuario, idConta);
            if (_pagamentoRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateMetodoPagamento(AtualizarMetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario, int idConta, int idMetodoPagamento)
        {
            var resultado = await _pagamentoRepository.UpdateMetodoPagamento(metodoPagamentoRequest.ToInfra(), idUsuario, idConta, idMetodoPagamento);
            if (_pagamentoRepository.HasNotifications)
                return false;

            return resultado;
        }
        
        public async Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento)
        {
            var resultado = await _pagamentoRepository.DeleteMetodoPagamento(idUsuario, idMetodoPagamento);
            if (_pagamentoRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
