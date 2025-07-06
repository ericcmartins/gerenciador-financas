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

      public async Task<List<Despesa?>> GetDespesas(int idUsuario, int? periodo)
        {
            var responseInfra = await _despesaRepository.GetDespesas(idUsuario, periodo);
            if (_despesaRepository.HasNotifications)
                return null;

            var despesas = responseInfra
                .Select(d => d.ToService())
                .ToList();

            return despesas;
        }
        public async Task<List<DespesaCategoria?>> GetDespesasPorCategoria(int idUsuario, int? periodo)
        {
            var responseInfra = await _despesaRepository.GetDespesasPorCategoria(idUsuario, periodo);
            if (_despesaRepository.HasNotifications)
                return null;

            var despesas = responseInfra
                .Select(d => d.ToService())
                .ToList();

            return despesas;
        }
        public async Task<List<DespesaConta?>> GetDespesasPorConta(int idUsuario, int? periodo)
        {
            var responseInfra = await _despesaRepository.GetDespesasPorConta(idUsuario, periodo);
            if (_despesaRepository.HasNotifications)
                return null;

            var despesas = responseInfra
                .Select(d => d.ToService())
                .ToList();

            return despesas;
        }
        public async Task<List<DespesaMetodoPagamento?>> GetDespesasPorMetodoPagamento(int idUsuario, int? periodo)
        {
            var responseInfra = await _despesaRepository.GetDespesasPorMetodoPagamento(idUsuario, periodo);
            if (_despesaRepository.HasNotifications)
                return null;

            var despesas = responseInfra
                .Select(d => d.ToService())
                .ToList();

            return despesas;
        }
        public async Task<Decimal?> GetTotalDespesasPeriodo(int idUsuario, int? periodo)
        {
            var responseInfra = await _despesaRepository.GetTotalDespesasPeriodo(idUsuario, periodo);
            if (_despesaRepository.HasNotifications)
                return null;

            return responseInfra;
        }
        
        public async Task<bool> InsertDespesa(DespesaRequestViewModel despesaRequest, int idUsuario, int idCategoria, int idConta, int idMetodoPagamento)
        {
            var resultado = await _despesaRepository.InsertDespesa(despesaRequest.ToInfra(), idUsuario, idCategoria, idConta, idMetodoPagamento);
            if (_despesaRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateDespesa(DespesaRequestViewModel despesaRequest, int idUsuario, int idDespesa, int idCategoria, int idConta, int idMetodoPagamento)        {
            var resultado = await _despesaRepository.UpdateDespesa(despesaRequest.ToInfra(), idUsuario, idDespesa, idCategoria, idConta, idMetodoPagamento);
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
