using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public interface IClienteService
    {
        public Task<DadosPessoais?> GetDadosPessoais(string cpf);
        public Task<TResultService<string>> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais);

        public Task<TResultService<string>> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, string cpf);

        public Task<TResultService<string>> UpdateSenha(string cpf, string senha);
        public Task<TResultService<string>> UpdateEmail(string cpf, string email);

        public Task<TResultService<string>> UpdateTelefone(string cpf, string telefone);

        public Task<TResultService<string>> DeleteConta(string cpf);
    }
}
