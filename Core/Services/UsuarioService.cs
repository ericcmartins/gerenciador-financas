using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthService _authService;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public UsuarioService(IUsuarioRepository usuarioRepository, 
                              NotificationPool notificationPool,
                              IAuthService authService)
        {
            _usuarioRepository = usuarioRepository;
            _notificationPool = notificationPool;
            _authService = authService;
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
            if (string.IsNullOrWhiteSpace(dadosPessoais.Email) ||
                string.IsNullOrWhiteSpace(dadosPessoais.Senha) ||
                string.IsNullOrWhiteSpace(dadosPessoais.Nome))
            {
                _notificationPool.AddNotification(400, "Campos obrigatórios não preenchidos");
                return false;
            }

            //var usuarioExistente = await _usuarioRepository.GetUsuarioPorEmail(dadosPessoais.Email);
            //if (usuarioExistente != null)
            //{
            //    _notificationPool.AddNotification(409, "Usuário já cadastrado com este e-mail");
            //    return false;
            //}

            var senhaHash = _authService.ComputeHash(dadosPessoais.Senha);

            var dadosInfra = new DadosPessoaisRequestInfra
            {
                Nome = dadosPessoais.Nome,
                Email = dadosPessoais.Email,
                Senha = senhaHash,
                DataNascimento = dadosPessoais.DataNascimento,
                Telefone = dadosPessoais.Telefone
            };

            var resultado = await _usuarioRepository.InsertDadosPessoais(dadosInfra);
            if (_usuarioRepository.HasNotifications)
                return false;

           return resultado;
        }

        public async Task<bool> UpdateDadosPessoais(DadosPessoaisRequestViewModel dadosPessoais, int idUsuario)
        {
            var usuarioAtual = await _usuarioRepository.GetDadosPessoais(idUsuario);
            if (usuarioAtual == null)
            {
                _notificationPool.AddNotification(404, "Usuário não encontrado");
                return false;
            }

            string senhaHash;

            if (!string.IsNullOrWhiteSpace(dadosPessoais.Senha))
                senhaHash = _authService.ComputeHash(dadosPessoais.Senha);
            else
                senhaHash = usuarioAtual.Senha; 

            var dadosInfra = new DadosPessoaisRequestInfra
            {
                Nome = string.IsNullOrWhiteSpace(dadosPessoais.Nome) ? null : dadosPessoais.Nome,
                Email = string.IsNullOrWhiteSpace(dadosPessoais.Email) ? null : dadosPessoais.Email,
                Senha = senhaHash,
                DataNascimento = dadosPessoais.DataNascimento,
                Telefone = string.IsNullOrWhiteSpace(dadosPessoais.Telefone) ? null : dadosPessoais.Telefone,
                RoleUsuario = null
            };


            var resultado = await _usuarioRepository.UpdateDadosPessoais(dadosInfra, idUsuario);

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
