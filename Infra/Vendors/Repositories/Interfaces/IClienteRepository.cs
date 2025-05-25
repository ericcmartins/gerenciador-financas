using gerenciador.financas.Infra.Vendors.Entities;
using Infra.Vendors.Entities;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IClienteRepository
    {
        public Task<TResultInfra<DadosPessoaisResponseInfra>> GetDadosPessoais(string cpf);
        public Task<TResultInfra<string>> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais);

        public Task<TResultInfra<string>> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, string cpf);

        public Task<TResultInfra<string>> UpdateSenha(string cpf, string senha);
        public Task<TResultInfra<string>> UpdateEmail(string cpf, string email);

        public Task<TResultInfra<string>> UpdateTelefone(string cpf, string telefone);

        public Task<TResultInfra<string>> DeleteConta(string cpf);
    }
}