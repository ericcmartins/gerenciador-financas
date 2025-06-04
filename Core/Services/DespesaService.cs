using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class DespesaService : IDespesaService
    {
        private readonly IDespesaRepository _despesaRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public DespesaService(IDespesaRepository despesaRepository, 
                              NotificationPool notificationPool)
        {
            _despesaRepository = despesaRepository;
            _notificationPool = notificationPool;
        }

      public async Task<List<Despesa?>> GetDespesas(int idUsuario)
        {
            var responseInfra = await _despesaRepository.GetDespesas(idUsuario);
            if (_despesaRepository.HasNotifications)
                return null;

            var despesas = responseInfra
                .Select(d => d.ToService())
                .ToList();

            return despesas;
        }

        public async Task<bool> InsertDespesa(DespesaRequestViewModel despesaRequest, int idUsuario, int idConta, int idCategoria, int idMetodoPagamento)
        {
            var resultado = await _despesaRepository.InsertDespesa(despesaRequest.ToInfra(), idUsuario, idConta, idCategoria, idMetodoPagamento);
            if (_despesaRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateDespesa(DespesaRequestViewModel despesaRequest, int idUsuario, int idDespesa, int idCategoria, int idConta, int idMetodoPagamento)        {
            var resultado = await _despesaRepository.UpdateDespesa(despesaRequest.ToInfra(), idUsuario, idDespesa, idConta, idCategoria, idMetodoPagamento);
            if (_despesaRepository.HasNotifications)
                return false;

            return resultado;
        }
        
        public async Task<bool> DeleteDespesa(int idUsuario, int idDespesa)
        {
            var resultado = await _despesaRepository.DeleteDespesa(idUsuario, idDespesa);
           if (_despesaRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
