using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IClienteRepository
    {
        public Task<DadosPessoaisResponseInfra?> GetDadosPessoais(string cpf);
        public Task<bool> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais);

        public Task<bool> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, string cpf);

        public Task<bool> UpdateSenha(string cpf, string senha);
        public Task<bool> UpdateEmail(string cpf, string email);

        public Task<bool> UpdateTelefone(string cpf, string telefone);

        public Task<bool> DeleteConta(string cpf);
    }
}