using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Application.Services
{
    public interface IUsuarioService : INotifiable
    {
        public Task<Usuario?> GetDadosPessoais(int idUsuario);
        public Task<bool> InsertCadastroUsuario(CadastrarUsuarioRequestViewModel cadastroUsuario);
        public Task<bool> UpdateDadosPessoais(AtualizarDadosCadastraisRequestViewModel dadosPessoais, int idUsuario);
        public Task<bool> AlterarSenha(string email, string novaSenha, string telefone);
        public Task<bool> DeleteConta(int idUsuario);
    }
}
