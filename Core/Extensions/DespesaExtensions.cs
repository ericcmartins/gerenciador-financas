using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class DespesaExtensions
    {
        public static Despesa ToService(this DespesaResponseInfra infra)
        {
            return new Despesa
            {
                Valor = infra.Valor,
                Descricao = infra.Descricao,
                DataDespesa = infra.DataDespesa,
                Conta = infra.Conta,
                Categoria = infra.Categoria,
                MetodoPagamento = infra.MetodoPagamento,
                IdDespesa = infra.IdDespesa,
            };
        }
        public static DespesaCategoria ToService(this DespesaPorCategoriaResponseInfra infra)
        {
            return new DespesaCategoria
            {
                Categoria = infra.Categoria,
                TotalDespesa = infra.TotalDespesa,
            };
        }
        public static DespesaConta ToService(this DespesaPorContaResponseInfra infra)
        {
            return new DespesaConta
            {
                NumeroConta = infra.NumeroConta,
                TotalDespesa = infra.TotalDespesa,
            };
        }
        public static DespesaMetodoPagamento ToService(this DespesaPorMetodoPagamentoResponseInfra infra)
        {
            return new DespesaMetodoPagamento
            {
                MetodoPagamento = infra.MetodoPagamento,
                TotalDespesa = infra.TotalDespesa,
            };
        }
        public static DespesaPorCategoriaResponseViewModel ToViewModel(this DespesaCategoria domain)
        {
            return new DespesaPorCategoriaResponseViewModel
            {
                Categoria = domain.Categoria,
                TotalDespesa = domain.TotalDespesa,
            };
        }
        public static DespesaPorContaResponseViewModel ToViewModel(this DespesaConta domain)
        {
            return new DespesaPorContaResponseViewModel
            {
                NumeroConta = domain.NumeroConta,
                TotalDespesa = domain.TotalDespesa,
            };
        }
        public static DespesaPorMetodoPagamentoResponseViewModel ToViewModel(this DespesaMetodoPagamento domain)
        {
            return new DespesaPorMetodoPagamentoResponseViewModel
            {
                MetodoPagamento = domain.MetodoPagamento,
                TotalDespesa = domain.TotalDespesa,
            };
        }
        public static DespesaResponseViewModel ToViewModel(this Despesa domain)
        {
            return new DespesaResponseViewModel
            {
                Valor = domain.Valor,
                Descricao = domain.Descricao,
                DataDespesa = domain.DataDespesa,
                Conta = domain.Conta,
                Categoria = domain.Categoria,
                MetodoPagamento = domain.MetodoPagamento,
                IdDespesa = domain.IdDespesa,
            };
        }
        public static DespesaRequestInfra ToInfra(this DespesaRequestViewModel viewModel)
        {
            return new DespesaRequestInfra
            {
                Valor = viewModel.Valor,
                Descricao = viewModel.Descricao,
                DataDespesa = viewModel.DataDespesa
            };
        }
    }
}

