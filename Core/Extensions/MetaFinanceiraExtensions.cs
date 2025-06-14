using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class MetaFinanceiraExtensions
    {
        public static MetaFinanceira ToService(this MetaFinanceiraResponseInfra infra)
        {
            return new MetaFinanceira
            {
                Descricao = infra.Descricao,
                ValorAlvo = infra.ValorAlvo,
                ValorAtual = infra.ValorAtual,
                DataInicio = infra.DataInicio,
                DataLimite = infra.DataLimite,
                Concluida = infra.Concluida
            };
        }

        public static MetaFinanceiraResponseViewModel ToViewModel(this MetaFinanceira domain)
        {
            return new MetaFinanceiraResponseViewModel
            {
                Descricao = domain.Descricao,
                ValorAlvo = domain.ValorAlvo,
                ValorAtual = domain.ValorAtual,
                DataInicio = domain.DataInicio,
                DataLimite = domain.DataLimite,
                Concluida = domain.Concluida
            };
        }

        public static MetaFinanceiraRequestInfra ToInfra(this MetaFinanceiraRequestViewModel viewModel)
        {
            return new MetaFinanceiraRequestInfra
            {
                Descricao = viewModel.Descricao,
                ValorAlvo = viewModel.ValorAlvo,
                ValorAtual = viewModel.ValorAtual,
                DataInicio = viewModel.DataInicio,
                DataLimite = viewModel.DataLimite
            };
        }
    }
}

