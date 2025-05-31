using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Notification;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly NotificationPool _notificationPool;
        public UsuarioService(IUsuarioRepository usuarioRepository, 
                              NotificationPool notificationPool)
        {
            _usuarioRepository = usuarioRepository;
            _notificationPool = notificationPool;
        }

        public async Task<DadosPessoais?> GetDadosPessoais(int idUsuario)
        {
            var responseInfra = await _usuarioRepository.GetDadosPessoais(idUsuario);

            if (responseInfra is null)
                return null;

            return responseInfra.ToService();
        }

        public async Task<bool> InsertDadosPessoais(DadosPessoaisRequestViewModel dadosPessoais)
        {
            var resultado = await _usuarioRepository.InsertDadosPessoais(dadosPessoais.ToInfra());
            return resultado;
        }

        public async Task<bool> UpdateDadosPessoais(DadosPessoaisRequestViewModel dadosPessoais, int idUsuario)
        {
            var resultado = await _usuarioRepository.UpdateDadosPessoais(dadosPessoais.ToInfra(), idUsuario);
            return resultado;
        }
        
        public async Task<bool> DeleteConta(int idUsuario)
        {
            var resultado = await _usuarioRepository.DeleteConta(idUsuario);
            return resultado;
        }
    }
}
