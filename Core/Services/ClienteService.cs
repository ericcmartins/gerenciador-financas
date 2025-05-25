using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Extensions;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Repositories;
using Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<TResultInfra<DadosPessoaisResponseService>> GetDadosPessoais(string cpf)
        {
            var responseInfra = await _clienteRepository.GetDadosPessoais(cpf);

            if (!responseInfra.Sucesso)
            {
                return TResultInfra<DadosPessoaisResponseService>.Fail(responseInfra.TipoErro, responseInfra.MensagemErro);
            }

            if (responseInfra.ObjetoClasse == null)
            {
                return TResultInfra<DadosPessoaisResponseService>.Fail(ErrosInfra.NotFound, "Usuário não encontrado.");
            }

            var responseService = MapperProfile.DadosPessoaisInfraToService(responseInfra.ObjetoClasse);

            return TResultInfra<DadosPessoaisResponseService>.Ok(responseService);
        }


        public async Task<TResultInfra<string>> InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais)
        {
            var dadosPessoaisInfra = MapperProfile.DadosPessoaiServiceToInfra(dadosPessoais);
            string resultado = await _clienteRepository.InsertDadosPessoais(dadosPessoaisInfra);
            return resultado;
        }

        public async Task<TResultInfra<string>> UpdateDadosPessoais(DadosPessoaisRequestInfra dadosPessoais, string cpf)
        {
            var dadosPessoaisInfra = MapperProfile.DadosPessoaiServiceToInfra(dadosPessoais);
            string resultado = await _clienteRepository.UpdateDadosPessoais(dadosPessoaisInfra, cpf);
            return resultado;
        }
        public async Task<TResultInfra<string>> UpdateSenha(string cpf, string senha)
        {
            string resultado = await _clienteRepository.UpdateSenha(cpf, senha);
            return resultado;
        }

        public async Task<TResultInfra<string>> UpdateEmail(string cpf, string email)
        {
            string resultado = await _clienteRepository.UpdateEmail(cpf, email);
            return resultado;
        }

        public async Task<TResultInfra<string>> UpdateTelefone(string cpf, string telefone)
        {
            string resultado = await _clienteRepository.UpdateTelefone(cpf, telefone);
            return resultado;
        }

        public async Task<TResultInfra<string>> DeleteConta(string cpf)
        {
            string resultado = await _clienteRepository.DeleteConta(cpf);
            return resultado;
        }
        }
    }
}
