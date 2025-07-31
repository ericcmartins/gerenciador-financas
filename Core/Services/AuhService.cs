using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace gerenciador.financas.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<AuthService> _logger;

        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;

        public AuthService(IConfiguration configuration,
                           IUsuarioRepository usuarioRepository,
                           NotificationPool notificationPool,
                           ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
            _notificationPool = notificationPool;
            _logger = logger;
        }
        public string CalcularHash(string senha)
        {
            using (var hash = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(senha);
                var hashBytes = hash.ComputeHash(passwordBytes);
                var builder = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public string GerarToken(string email, string role, DateTime expiracao)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("username", email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(issuer, audience, claims, null, expiracao, credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponseViewModel> RealizarLogin(string email, string senha)
        {
            var usuario = await _usuarioRepository.GetUsuarioPorEmail(email);

            if (usuario == null)
            {
                _notificationPool.AddNotification(400, "Email inválido");
                _logger.LogWarning("Falha no login para {Email}: Email inválido.", email);
                return null;
            }

            var senhaHash = CalcularHash(senha);

            if (usuario.SenhaHash != senhaHash)
            {
                _notificationPool.AddNotification(400, "Senha incorreta");
                _logger.LogWarning("Falha no login para {Email}: Senha incorreta.", email);
                return null;
            }

            var roleUsuario = usuario.RoleUsuario;
            var expiracao = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:Duracao"]));
            var token = GerarToken(email, roleUsuario, expiracao);

            _logger.LogInformation("Login para o usuário {IdUsuario} ({Email}) realizado com sucesso. Token gerado.", usuario.IdUsuario, email);

            return new LoginResponseViewModel
            {
                Token = token,
                Expiracao = expiracao,
                Tipo = "Bearer",
                IdUsuario = usuario.IdUsuario
            };
        }

    }

}
