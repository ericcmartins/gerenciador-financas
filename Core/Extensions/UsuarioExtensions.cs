using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class DadosPessoaisExtensions
    {
        public static Usuario ToService(this DadosPessoaisResponseInfra infra)
        {
            return new Usuario
            {
                Nome = infra.Nome,
                Email = infra.Email,
                Senha = infra.SenhaHash,
                Role = infra.RoleUsuario,
                DataNascimento = infra.DataNascimento,
                Telefone = infra.Telefone
            };
        }

        public static DadosPessoaisResponseViewModel ToViewModel(this Usuario domain)
        {
            return new DadosPessoaisResponseViewModel
            {
                Nome = domain.Nome,
                Email = domain.Email,
                Senha = domain.Senha,
                DataNascimento = domain.DataNascimento,
                Telefone = domain.Telefone
            };
        }

    }
}

