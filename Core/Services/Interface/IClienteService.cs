using gerenciador.financas.Core.Entities;

namespace gerenciador.financas.Core.Services
{
    public interface IClienteService
    {
        public string InserirDadosCadastrais(DadosPessoaisRequestService dadosPessoais);
    }
} 
