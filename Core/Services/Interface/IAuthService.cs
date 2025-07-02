using gerenciador.financas.API.ViewModel.Cliente;

namespace gerenciador.financas.Application.Services
{
    public interface IAuthService
    {
        public string ComputeHash(string password);
        public string GenerateToken(string email, string role, DateTime expiracao);
        public Task<LoginResponseViewModel> Login(string email, string senha);
    }
}
