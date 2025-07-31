using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class PagamentoExtensions
    {
        public static MetodoPagamento ToService(this MetodoPagamentoResponseInfra infra)
        {
            return new MetodoPagamento
            {
                Nome = infra.Nome,
                TipoMetodoPagamento = infra.TipoMetodoPagamento,
                NumeroConta = infra.NumeroConta,
                Limite = infra.Limite,
                IdTipoMetodo = infra.IdTipoMetodo,
                IdMetodo = infra.IdMetodo,
                IdConta = infra.IdConta,
                IdUsuario = infra.IdUsuario,
            };
        }

        public static MetodoPagamentoResponseViewModel ToViewModel(this MetodoPagamento domain)
        {
            return new MetodoPagamentoResponseViewModel
            {
                Nome = domain.Nome,
                TipoMetodoPagamento = domain.TipoMetodoPagamento,
                NumeroConta = domain.NumeroConta,
                Limite = domain.Limite,
                IdTipoMetodo = domain.IdTipoMetodo,
                IdMetodo = domain.IdMetodo,
                IdConta = domain.IdConta,
                IdUsuario = domain.IdUsuario,
            };
        }

        public static CadastrarMetodoPagamentoRequestInfra ToInfra(this CadastrarMetodoPagamentoRequestViewModel viewModel)
        {
            return new CadastrarMetodoPagamentoRequestInfra
            {
                Nome = viewModel.Nome,
                IdTipoMetodo = viewModel.IdTipoMetodo,
                Limite = viewModel.Limite
            };
        }

        public static AtualizarMetodoPagamentoRequestInfra ToInfra(this AtualizarMetodoPagamentoRequestViewModel viewModel)
        {
            return new AtualizarMetodoPagamentoRequestInfra
            {
                Nome = viewModel.Nome,
                IdTipoMetodo = viewModel.IdTipoMetodo,
                Limite = viewModel.Limite
            };
        }
    }
}


