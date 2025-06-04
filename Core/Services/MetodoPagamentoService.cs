using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class MetodoPagamentoService : IMetodoPagamentoService
    {
        private readonly IMetodoPagamentoRepository _metodoPagamentoRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public MetodoPagamentoService(IMetodoPagamentoRepository metodoPagamentoRepository, 
                              NotificationPool notificationPool)
        {
            _metodoPagamentoRepository = metodoPagamentoRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<MetodoPagamento?>> GetMetodosPagamentoUsuario(int idUsuario)
        {
            var responseInfra = await _metodoPagamentoRepository.GetMetodosPagamentoUsuario(idUsuario);
            if (HasNotifications)
                return null;

            var metodosPagamento = responseInfra
                .Select(m => m.ToService())
                .ToList();

            return metodosPagamento;
        }

        public async Task<bool> InsertMetodoPagamento(MetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario, int idConta)
        {
            var resultado = await _metodoPagamentoRepository.InsertMetodoPagamento(metodoPagamentoRequest.ToInfra(), idUsuario, idConta);
            if (_metodoPagamentoRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateMetodoPagamento(MetodoPagamentoRequestViewModel metodoPagamentoRequest, int idUsuario, int idConta, int idMetodoPagamento)
        {
            var resultado = await _metodoPagamentoRepository.UpdateMetodoPagamento(metodoPagamentoRequest.ToInfra(), idUsuario, idConta, idMetodoPagamento);
            if (_metodoPagamentoRepository.HasNotifications)
                return false;

            return resultado;
        }
        
        public async Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento)
        {
            var resultado = await _metodoPagamentoRepository.DeleteMetodoPagamento(idUsuario, idMetodoPagamento);
            if (_metodoPagamentoRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
