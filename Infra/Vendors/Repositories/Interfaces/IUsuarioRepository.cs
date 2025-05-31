using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IUsuarioRepository
    {
        public Task<DadosPessoaisResponseInfra?> GetDadosPessoais(int idUsuario);
        public Task<bool> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais);
        public Task<bool> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, int idUsuario);
        public Task<bool> DeleteConta(int idUsuario);
    }
}