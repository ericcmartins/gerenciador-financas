using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Notification;

namespace gerenciador.financas.Application.Services
{
    public interface IUsuarioService
    {
        public Task<DadosPessoais?> GetDadosPessoais(int idUsuario);
        public Task<bool> InsertDadosPessoais(DadosPessoaisRequestViewModel dadosPessoais);
        public Task<bool> UpdateDadosPessoais(DadosPessoaisRequestViewModel dadosPessoais, int idUsuario);
        public Task<bool> DeleteConta(int idUsuario);
    }
}
