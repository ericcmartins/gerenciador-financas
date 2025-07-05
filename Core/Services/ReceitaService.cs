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

        public async Task<List<Receita?>> GetReceitas(int idUsuario, int? periodo)
        {
            var responseInfra = await _receitaRepository.GetReceitas(idUsuario, periodo);
            if (_receitaRepository.HasNotifications)
                return null;

            var receitas = responseInfra
                .Select(r => r.ToService())
                .ToList();

            return receitas;
        }
        public async Task<List<ReceitaCategoria?>> GetReceitasPorCategoria(int idUsuario, int? periodo)
        {
            var responseInfra = await _receitaRepository.GetReceitasPorCategoria(idUsuario, periodo);
            if (_receitaRepository.HasNotifications)
                return null;

            var receitas = responseInfra
                .Select(r => r.ToService())
                .ToList();

            return receitas;
        }

        public async Task<List<ReceitaConta?>> GetReceitasPorConta(int idUsuario, int? periodo)
        {
            var responseInfra = await _receitaRepository.GetReceitasPorConta(idUsuario, periodo);
            if (_receitaRepository.HasNotifications)
                return null;

            var receitas = responseInfra
                .Select(r => r.ToService())
                .ToList();

            return receitas;
        }
        public async Task<Decimal?> GetReceitasTotalPorPeriodo(int idUsuario, int? periodo)
        {
            var responseInfra = await _receitaRepository.GetReceitasTotalPorPeriodo(idUsuario, periodo);
            if (_receitaRepository.HasNotifications)
                return null;

            return responseInfra;
        }

        public async Task<bool> InsertReceita(ReceitaRequestViewModel receitaRequest, int idUsuario, int idCategoria, int idConta)
        {
            var resultado = await _receitaRepository.InsertReceita(receitaRequest.ToInfra(), idUsuario, idCategoria, idConta);
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
