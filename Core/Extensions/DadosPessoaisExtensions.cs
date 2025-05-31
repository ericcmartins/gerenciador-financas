using gerenciador.financas.API.ViewModel.Cliente;
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
                Email = infra.Email,
                Senha = infra.Senha,
                DataNascimento = infra.DataNascimento,
                Telefone = infra.Telefone
            };
        }

        public static DadosPessoaisResponseViewModel ToViewModel(this DadosPessoais service)
        {
            return new DadosPessoaisResponseViewModel
            {
                Nome = service.Nome,
                Email = service.Email,
                Senha = service.Senha,
                DataNascimento = service.DataNascimento,
                Telefone = service.Telefone
            };
        }

        public static DadosPessoaisRequestInfra ToInfra(this DadosPessoaisRequestViewModel viewModel)
        {
            return new DadosPessoaisRequestInfra
            {
                Nome = viewModel.Nome,
                Email = viewModel.Email,
                Senha = viewModel.Senha,
                DataNascimento= viewModel.DataNascimento,
                Telefone = viewModel.Telefone
            };
        }
    }
}

