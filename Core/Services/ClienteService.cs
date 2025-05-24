using gerenciador.financas.Infra.Vendors.Repositories;
using gerenciador.financas.Core.Entities;
using System.Reflection.Metadata.Ecma335;
using gerenciador.financas.Extensions;

namespace gerenciador.financas.Core.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;

        }
        public string InserirDadosCadastrais(DadosPessoaisRequestService dadosPessoais)
        {
            //transformar dados pessoais service para infra
            var dadosPessoaisInfra = MapperProfile.ToInfraRequest(dadosPessoais);
            var responseInfra = _clienteRepository.ConsultaDadosCadastrais(dadosPessoaisInfra);
            return new ClienteResponseService();

        } 

    }
}

