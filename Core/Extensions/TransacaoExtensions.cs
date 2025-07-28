using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class TransacaoExtensions
    {
        public static MovimentacaoFinanceira ToService(this MovimentacaoFinanceiraResponseInfra infra)
        {
            return new MovimentacaoFinanceira
            {
                TipoMovimentacao = infra.TipoMovimentacao,
                Valor = infra.Valor,
                DataMovimentacao = infra.DataMovimentacao,
                Descricao = infra.Descricao,
                NumeroContaDestino = infra.NumeroContaDestino,
                NumeroContaOrigem = infra.NumeroContaOrigem,
                IdMovimentacao = infra.IdMovimentacao
            };
        }

        public static SaldoContas ToService (this SaldoPorContaResponseInfra infra)
        {
            return new SaldoContas
            {
                NumeroConta = infra.NumeroConta,
                Instituicao = infra.Instituicao,
                Tipo = infra.Tipo,
                SaldoConta = infra.SaldoConta
            };
        }

        public static SaldoPorContaResponseViewModel ToViewModel(this SaldoContas infra)
        {
            return new SaldoPorContaResponseViewModel
            {
                NumeroConta = infra.NumeroConta,
                Instituicao = infra.Instituicao,
                Tipo = infra.Tipo,
                SaldoConta = infra.SaldoConta
            };
        }

        public static SaldoTotalContas ToService(this SaldoTotalUsuarioResponseInfra infra)
        {
            return new SaldoTotalContas
            {
                SaldoTotal = infra.SaldoTotal
            };
        }

        public static SaldoTotalUsuarioResponseViewModel ToViewModel(this SaldoTotalContas domain)
        {
            return new SaldoTotalUsuarioResponseViewModel
            {
                SaldoTotal = domain.SaldoTotal
            };
        }

        public static MovimentacaoFinanceiraResponseViewModel ToViewModel(this MovimentacaoFinanceira domain)
        {
            return new MovimentacaoFinanceiraResponseViewModel
            {
                TipoMovimentacao = domain.TipoMovimentacao,
                Valor = domain.Valor,
                DataMovimentacao = domain.DataMovimentacao,
                Descricao = domain.Descricao,
                NumeroContaDestino = domain.NumeroContaDestino,
                NumeroContaOrigem = domain.NumeroContaOrigem,
                IdMovimentacao = domain.IdMovimentacao

            };
        }
        public static CadastrarTransacaoRequestInfra ToInfra (this CadastrarTransacaoRequestViewModel viewModel)
        {
            return new CadastrarTransacaoRequestInfra
            {
                TipoMovimentacao = viewModel.TipoMovimentacao,
                Valor = viewModel.Valor,
                DataMovimentacao = viewModel.DataMovimentacao,
                Descricao = viewModel.Descricao
            };
        }
        public static AtualizarTransacaoRequestInfra ToInfra(this AtualizarTransacaoRequestViewModel viewModel)
        {
            return new AtualizarTransacaoRequestInfra
            {
                Valor = viewModel.Valor,
                DataMovimentacao = viewModel.DataMovimentacao,
                Descricao = viewModel.Descricao
            };
        }
    }
}

