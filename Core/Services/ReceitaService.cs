using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class ReceitaService : IReceitaService
    {
        private readonly IReceitaRepository _receitaRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public ReceitaService(IReceitaRepository receitaRepository, 
                              NotificationPool notificationPool)
        {
            _receitaRepository = receitaRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<Receita?>> GetReceitas(int idUsuario)
        {
            var responseInfra = await _receitaRepository.GetReceita(idUsuario);
            if (_receitaRepository.HasNotifications)
                return null;

            var receitas = responseInfra
                .Select(r => r.ToService())
                .ToList();

            return receitas;
        }

        public async Task<bool> InsertReceita(ReceitaRequestViewModel receitaRequest, int idUsuario, int idConta, int idCategoria)
        {
            var resultado = await _receitaRepository.InsertReceita(receitaRequest.ToInfra(), idUsuario, idConta, idCategoria);
            if (_receitaRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateReceita(ReceitaRequestViewModel receitaRequest, int idUsuario, int idReceita, int idCategoria, int idConta)
        {
            var resultado = await _receitaRepository.UpdateReceita(receitaRequest.ToInfra(), idUsuario, idReceita, idCategoria, idConta);
            if (_receitaRepository.HasNotifications)
                return false;

            return resultado;
        }
        
        public async Task<bool> DeleteReceita(int idUsuario, int idReceita)
        {
            var resultado = await _receitaRepository.DeleteReceita(idUsuario, idReceita);
           if (_receitaRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
