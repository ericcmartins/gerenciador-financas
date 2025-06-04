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
                Data = infra.Data,
                Recorrente = infra.Recorrente,
                Frequencia = infra.Frequencia,
            };
        }

        public static DespesaResponseViewModel ToViewModel(this Despesa domain)
        {
            return new DespesaResponseViewModel
            {
                Valor = domain.Valor,
                Descricao = domain.Descricao,
                Data = domain.Data,
                Recorrente = domain.Recorrente,
                Frequencia = domain.Frequencia,
            };
        }
        public static DespesaRequestInfra ToInfra(this DespesaRequestViewModel viewModel)
        {
            return new DespesaRequestInfra
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

