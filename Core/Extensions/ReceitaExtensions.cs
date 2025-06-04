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
                Data = infra.Data,
                Recorrente = infra.Recorrente,
                Frequencia = infra.Frequencia,
            };
        }

        public static ReceitaResponseViewModel ToViewModel(this Receita domain)
        {
            return new ReceitaResponseViewModel
            {
                Valor = domain.Valor,
                Descricao = domain.Descricao,
                Data = domain.Data,
                Recorrente = domain.Recorrente,
                Frequencia = domain.Frequencia,
            };
        }
        public static ReceitaRequestInfra ToInfra(this ReceitaRequestViewModel viewModel)
        {
            return new ReceitaRequestInfra
            {
                Valor = viewModel.Valor,
                Descricao = viewModel.Descricao,
                Data = viewModel.Data,
                Recorrente = viewModel.Recorrente,
                Frequencia = viewModel.Frequencia,

            };
        }
    }
}

