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
                IdDespesa = infra.IdDespesa,                
                Valor = infra.Valor,
                Descricao = infra.Descricao,
                DataDespesa = infra.DataDespesa,
                IdUsuario = infra.IdUsuario,
                Categoria = infra.Categoria,
                Instituicao = infra.Instituicao,
                TipoConta = infra.TipoConta,
                MetodoPagamento = infra.MetodoPagamento,
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
                Instituicao = infra.Instituicao,
                TipoConta = infra.TipoConta,
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
                Instituicao = domain.Instituicao,
                TipoConta = domain.TipoConta,
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
                IdDespesa = domain.IdDespesa,
                Valor = domain.Valor,
                Descricao = domain.Descricao,
                DataDespesa = domain.DataDespesa,
                IdUsuario = domain.IdUsuario,
                Categoria = domain.Categoria,
                Instituicao = domain.Instituicao,
                TipoConta = domain.TipoConta,
                MetodoPagamento = domain.MetodoPagamento,
            };
        }
        public static CadastrarDespesaRequestInfra ToInfra(this CadastrarDespesaRequestViewModel viewModel)
        {
            return new CadastrarDespesaRequestInfra
            {
                Valor = viewModel.Valor,
                Descricao = viewModel.Descricao,
                DataDespesa = viewModel.DataDespesa
            };
        }
        public static AtualizarDespesaRequestInfra ToInfra(this AtualizarDespesaRequestViewModel viewModel)
        {
            return new AtualizarDespesaRequestInfra
            {
                Valor = viewModel.Valor,
                Descricao = viewModel.Descricao,
                DataDespesa = viewModel.DataDespesa
            };
        }
    }
}

