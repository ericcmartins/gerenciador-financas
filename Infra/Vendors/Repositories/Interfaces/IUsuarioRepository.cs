using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IUsuarioRepository : INotifiable
    {
        public Task<DadosPessoaisResponseInfra?> GetDadosPessoais(int idUsuario);
        public Task<bool> InsertCadastroUsuario(CadastrarUsuarioRequestInfra dadosCadastro);
        public Task<bool> UpdateDadosPessoais(AtualizarDadosCadastraisRequestInfra dadosPessoais, int idUsuario);
        public Task<bool> DeleteConta(int idUsuario);
        public Task<DadosPessoaisResponseInfra?> GetUsuarioPorEmail(string email);
    }
}