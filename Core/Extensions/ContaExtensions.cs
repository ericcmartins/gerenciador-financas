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
                Tipo = infra.Tipo,
                Instituicao = infra.Instituicao,
                IdConta = infra.IdConta

            };
        }

        public static ContaResponseViewModel ToViewModel(this Conta domain)
        {
            return new ContaResponseViewModel
            {
                NumeroConta = domain.NumeroConta,
                Tipo = domain.Tipo,
                Instituicao = domain.Instituicao,
                IdConta = domain.IdConta
            };
        }

        public static ContaRequestInfra ToInfra(this ContaRequestViewModel viewModel)
        {
            return new ContaRequestInfra
            {
                NumeroConta = viewModel.NumeroConta,
                Tipo = viewModel.Tipo,
                Instituicao = viewModel.Instituicao

            };
        }
    }
}

