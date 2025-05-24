using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IClienteRepository
    {
        public string InsertDadosPessoais(DadosPessoaisRequestInfra dadosPessoais);
    }
}