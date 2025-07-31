using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IAuthService : INotifiable
    {
        public string CalcularHash(string senha);
        public string GerarToken(string email, string role, DateTime expiracao);
        public Task<LoginResponseViewModel> RealizarLogin(string email, string senha);
    }
}
