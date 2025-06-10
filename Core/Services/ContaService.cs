using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class ContaService : IContaService
    {
        private readonly IContaRepository _contaRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public ContaService(IContaRepository contaRepository, 
                              NotificationPool notificationPool)
        {
            _contaRepository = contaRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<Conta>?> GetContas(int idUsuario)
        {
            var responseInfra = await _contaRepository.GetContas(idUsuario);
            if (HasNotifications)
                return null;

            var contas = responseInfra
                .Select(c => c.ToService())
                .ToList();

            return contas;
        }

        public async Task<bool> InsertConta(ContaRequestViewModel conta, int idUsuario)
        {
            var resultado = await _contaRepository.InsertConta(conta.ToInfra(), idUsuario);
            if (_contaRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateConta(ContaRequestViewModel conta, int idUsuario, int idConta)
        {
            var resultado = await _contaRepository.UpdateConta(conta.ToInfra(), idUsuario, idConta);
            if (_contaRepository.HasNotifications)
                return false;

            return resultado;
        }
        
        public async Task<bool> DeleteConta(int idConta, int idUsuario)
        {
            var resultado = await _contaRepository.DeleteConta(idConta, idUsuario);
            if (_contaRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
