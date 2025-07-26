using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class ContaExtensions
    {
        public static Conta ToService(this ContaResponseInfra infra)
        {
            return new Conta
            {
                NumeroConta = infra.NumeroConta,
                TipoConta = infra.TipoConta,
                Instituicao = infra.Instituicao,
                IdConta = infra.IdConta,
                IdUsuario = infra.IdUsuario,
            };
        }

        public static ContaResponseViewModel ToViewModel(this Conta domain)
        {
            return new ContaResponseViewModel
            {
                NumeroConta = domain.NumeroConta,
                TipoConta = domain.TipoConta,
                Instituicao = domain.Instituicao,
                IdConta = domain.IdConta,
                IdUsuario = domain.IdUsuario,
            };
        }

        public static InserirContaRequestInfra ToInfra(this InserirContaRequestViewModel viewModel)
        {
            return new InserirContaRequestInfra
            {
                IdTipoConta = viewModel.IdTipoConta,
                Instituicao = viewModel.Instituicao,
                NumeroConta = viewModel.NumeroConta
            };
        }

        public static AtualizarContaRequestInfra ToInfra(this AtualizarContaRequestViewModel viewModel)
        {
            return new AtualizarContaRequestInfra
            {
                IdTipoConta = viewModel.IdTipoConta,
                Instituicao = viewModel.Instituicao,
                NumeroConta = viewModel.NumeroConta
            };
        }
    }
}

