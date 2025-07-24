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

        public async Task<bool> InsertCadastroUsuario(CadastrarUsuarioRequestViewModel dadosCadastro)
        {
            var usuarioExistente = await _usuarioRepository.GetUsuarioPorEmail(dadosCadastro.Email);
            if (usuarioExistente != null)
            {
                _notificationPool.AddNotification(409, "Usuário já cadastrado com este e-mail");
                return false;
            }

            var senhaHash = _authService.CalcularHash(dadosCadastro.Senha);

            var dadosInfra = new CadastrarUsuarioRequestInfra
            {
                Nome = dadosCadastro.Nome,
                Email = dadosCadastro.Email,
                SenhaHash = senhaHash,
                DataNascimento = dadosCadastro.DataNascimento,
                Telefone = dadosCadastro.Telefone
            };

            var resultado = await _usuarioRepository.InsertCadastroUsuario(dadosInfra);
            if (_usuarioRepository.HasNotifications)
                return false;

           return resultado;
        }

        public async Task<bool> UpdateDadosPessoais(AtualizarDadosCadastraisRequestViewModel dadosPessoais, int idUsuario)
        {
            string? senhaHash = null;

            if (!string.IsNullOrWhiteSpace(dadosPessoais.Senha))
                senhaHash = _authService.CalcularHash(dadosPessoais.Senha);

            var dadosRequestInfra = new AtualizarDadosCadastraisRequestInfra
            {
                Nome = string.IsNullOrWhiteSpace(dadosPessoais.Nome) ? null : dadosPessoais.Nome,
                Email = string.IsNullOrWhiteSpace(dadosPessoais.Email) ? null : dadosPessoais.Email,
                SenhaHash = senhaHash,
                DataNascimento = dadosPessoais.DataNascimento == DateTime.MinValue ? null : dadosPessoais.DataNascimento,
                Telefone = string.IsNullOrWhiteSpace(dadosPessoais.Telefone) ? null : dadosPessoais.Telefone,
            };

            var resultado = await _usuarioRepository.UpdateDadosPessoais(dadosRequestInfra, idUsuario);

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
