using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class MetaFinanceiraService : IMetaFinanceiraService
    {
        private readonly IMetaFinanceiraRepository _metaFinanceiraRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public MetaFinanceiraService(IMetaFinanceiraRepository metaFinanceiraRepository, 
                              NotificationPool notificationPool)
        {
            _metaFinanceiraRepository = metaFinanceiraRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<MetaFinanceira?>> GetMetasFinanceiras(int idUsuario)
        {
            var responseInfra = await _metaFinanceiraRepository.GetMetasFinanceiras(idUsuario);
            if (_metaFinanceiraRepository.HasNotifications)
                return null;

            var metaFinanceira = responseInfra
                .Select(mf => mf.ToService())
                .ToList();

            return metaFinanceira;
        }

        public async Task<bool> InsertMetaFinanceira(MetaFinanceiraRequestViewModel metaFinanceiraRequest, int idUsuario)
        {
            var resultado = await _metaFinanceiraRepository.InsertMetaFinanceira(metaFinanceiraRequest.ToInfra(), idUsuario);
            if (_metaFinanceiraRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateMetaFinanceira(MetaFinanceiraRequestViewModel metaFinanceiraRequest, int idUsuario, int idMetaFinanceira)
        {
            var resultado = await _metaFinanceiraRepository.UpdateMetaFinanceira(metaFinanceiraRequest.ToInfra(), idUsuario, idMetaFinanceira);
            if (_metaFinanceiraRepository.HasNotifications)
                return false;

            return resultado;
        }
        
        public async Task<bool> DeleteMetaFinanceira(int idMetaFinanceira, int idUsuario)
        {
            var resultado = await _metaFinanceiraRepository.DeleteMetaFinanceira(idMetaFinanceira, idUsuario);
            if (_metaFinanceiraRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
