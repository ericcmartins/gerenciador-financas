using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public UsuarioService(IUsuarioRepository usuarioRepository, 
                              NotificationPool notificationPool)
        {
            _usuarioRepository = usuarioRepository;
            _notificationPool = notificationPool;
        }

        public async Task<Usuario?> GetDadosPessoais(int idUsuario)
        {
            var responseInfra = await _usuarioRepository.GetDadosPessoais(idUsuario);
            if (_usuarioRepository.HasNotifications)
                return null;

            return responseInfra.ToService();
        }

        public async Task<bool> InsertDadosPessoais(DadosPessoaisRequestViewModel dadosPessoais)
        {
            var resultado = await _usuarioRepository.InsertDadosPessoais(dadosPessoais.ToInfra());
            if (_usuarioRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> UpdateDadosPessoais(DadosPessoaisRequestViewModel dadosPessoais, int idUsuario)
        {
            var resultado = await _usuarioRepository.UpdateDadosPessoais(dadosPessoais.ToInfra(), idUsuario);
            if (_usuarioRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> DeleteConta(int idUsuario)
        {
            var resultado = await _usuarioRepository.DeleteConta(idUsuario);
            if (_usuarioRepository.HasNotifications)
                return false;
            return resultado;
        }
    }
}
