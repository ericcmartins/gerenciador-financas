using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public CategoriaService(ICategoriaRepository categoriaRepository, 
                              NotificationPool notificationPool)
        {
            _categoriaRepository = categoriaRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<Categoria>?> GetCategorias(int idUsuario)
        {
            var responseInfra = await _categoriaRepository.GetCategorias(idUsuario);
            if (HasNotifications)
                return null;

            var categoriasUsuario = responseInfra
            .Select(c => c.ToService())
            .ToList();

            return categoriasUsuario;
        }

        public async Task<bool> InsertCategoria(CategoriaRequestViewModel categoriaRequest, int idUsuario)
        {
            var resultado = await _categoriaRepository.InsertCategoria(categoriaRequest.ToInfra(), idUsuario);
            return resultado;
        }

        public async Task<bool> UpdateCategoria(CategoriaRequestViewModel categoriaRequest, int idUsuario)
        {
            var resultado = await _categoriaRepository.UpdateCategoria(categoriaRequest.ToInfra(), idUsuario);
            if (_categoriaRepository.HasNotifications)
                return false;

            return resultado;
        }
        
        public async Task<bool> DeleteCategoria(string nomeCategoria, int idUsuario)
        {
            var resultado = await _categoriaRepository.DeleteCategoria(nomeCategoria, idUsuario);
            if (_categoriaRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
