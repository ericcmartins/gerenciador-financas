using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class ReceitaExtensions
    {
        public static Receita ToService(this ReceitaResponseInfra infra)
        {
            return new Receita
            {
                Valor = infra.Valor,
                Descricao = infra.Descricao,
                DataReceita = infra.DataReceita,
                Conta = infra.Conta,
                Categoria = infra.Categoria                
            };
        }

        public static ReceitaCategoria ToService(this ReceitaPorCategoriaResponseInfra infra)
        {
            return new ReceitaCategoria
            {
                Categoria = infra.Categoria,
                TotalReceita = infra.TotalReceita
            };
        }
        public static ReceitaConta ToService(this ReceitaPorContaResponseInfra infra)
        {
            return new ReceitaConta
            {
                NumeroConta = infra.NumeroConta,
                TotalReceita = infra.TotalReceita
            };
        }

        public static ReceitaPorCategoriaResponseViewModel ToViewModel(this ReceitaCategoria domain)
        {
            return new ReceitaPorCategoriaResponseViewModel
            {
                Categoria = domain.Categoria,
                TotalReceita = domain.TotalReceita,
            };
        }
        public static ReceitaPorContaResponseViewModel ToViewModel(this ReceitaConta domain)
        {
            return new ReceitaPorContaResponseViewModel
            {
                NumeroConta = domain.NumeroConta,
                TotalReceita = domain.TotalReceita,
            };
        }

        public static ReceitaResponseViewModel ToViewModel(this Receita domain)
        {
            return new ReceitaResponseViewModel
            {
                Valor = domain.Valor,
                Descricao = domain.Descricao,
                DataReceita = domain.DataReceita,
                Conta = domain.Conta,
                Categoria = domain.Categoria
            };
        }
        public static ReceitaRequestInfra ToInfra(this ReceitaRequestViewModel viewModel)
        {
            return new ReceitaRequestInfra
            {
                Valor = viewModel.Valor,
                Descricao = viewModel.Descricao,
                DataReceita = viewModel.DataReceita,

            };
        }
    }
}

