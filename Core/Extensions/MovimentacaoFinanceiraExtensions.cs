using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class MovimentacaoFinanceiraExtensions
    {
        public static MovimentacaoFinanceira ToService(this MovimentacaoFinanceiraResponseInfra infra)
        {
            return new MovimentacaoFinanceira
            {
                TipoMovimentacao = infra.TipoMovimentacao,
                Valor = infra.Valor,
                Descricao = infra.Descricao,
                Data = infra.Data
            };
        }

        public static MovimentacaoFinanceiraResponseViewModel ToViewModel(this MovimentacaoFinanceira domain)
        {
            return new MovimentacaoFinanceiraResponseViewModel
            {
                TipoMovimentacao = domain.TipoMovimentacao,
                Valor = domain.Valor,
                Descricao = domain.Descricao,
                Data = domain.Data
            };
        }
        public static MovimentacaoFinanceiraRequestInfra ToInfra(this MovimentacaoFinanceiraRequestViewModel viewModel)
        {
            return new MovimentacaoFinanceiraRequestInfra
            {
                TipoMovimentacao = viewModel.TipoMovimentacao,
                Valor = viewModel.Valor,
                Descricao = viewModel.Descricao,
                Data = viewModel.Data
            };
        }
    }
}

