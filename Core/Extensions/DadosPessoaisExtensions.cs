using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class DadosPessoaisExtensions
    {
        public static DadosPessoais ToService(this DadosPessoaisResponseInfra infra)
        {
            return new DadosPessoais
            {
                Nome = infra.Nome,
                Cpf = infra.Cpf,
                DataNascimento = infra.DataNascimento,
                Telefone = infra.Telefone
            };
        }

        public static DadosPessoaisResponseViewModel ToViewModel(this DadosPessoais service)
        {
            return new DadosPessoaisResponseViewModel
            {
                Nome = service.Nome,
                Cpf = service.Cpf,
                DataNascimento = service.DataNascimento,
                Telefone = service.Telefone
            };
        }
    }
}

