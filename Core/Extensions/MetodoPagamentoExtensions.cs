using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class MetodoPagamentoExtensions
    {
        public static MetodoPagamento ToService(this MetodoPagamentoResponseInfra infra)
        {
            return new MetodoPagamento
            {
                Nome = infra.Nome,
                Descricao = infra.Descricao,
                Limite = infra.Limite,
                Tipo = infra.Tipo,
                NumeroConta = infra.NumeroConta
            };
        }

        public static MetodoPagamentoResponseViewModel ToViewModel(this MetodoPagamento domain)
        {
            return new MetodoPagamentoResponseViewModel
            {
                Nome = domain.Nome,
                Descricao = domain.Descricao,
                Limite = domain.Limite,
                Tipo = domain.Tipo,
            };
        }

        public static MetodoPagamentoRequestInfra ToInfra(this MetodoPagamentoRequestViewModel viewModel)
        {
            return new MetodoPagamentoRequestInfra
            {
                Nome = viewModel.Nome,
                Descricao = viewModel.Descricao,
                Limite = viewModel.Limite,
                Tipo = viewModel.Tipo,

            };
        }
    }
}

