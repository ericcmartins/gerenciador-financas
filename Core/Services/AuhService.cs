using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Infra.Vendors.Repositories;
using Microsoft.Extensions.Configuration;
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

        public AuthService(IConfiguration configuration,
                           IUsuarioRepository usuarioRepository)
        {
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
        }
        public string ComputeHash(string password)
        {
            using (var hash = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = hash.ComputeHash(passwordBytes);
                var builder = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public string GenerateToken(string email, string role, DateTime expiracao)
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


        public async Task<LoginResponseViewModel> Login(string email, string senha)
        {
            var usuario = await _usuarioRepository.GetUsuarioPorEmail(email);

            if (usuario == null)
                throw new UnauthorizedAccessException("Usuário não encontrado");

            var senhaHash = ComputeHash(senha);

            if (usuario.Senha != senhaHash)
                throw new UnauthorizedAccessException("Senha incorreta");

            var expiracao = DateTime.UtcNow.AddHours(2);
            var token = GenerateToken(email, "User", expiracao);

            return new LoginResponseViewModel
            {
                Token = token,
                Expiracao = expiracao,
                Tipo = "Bearer"
            };
        }

    }

}
