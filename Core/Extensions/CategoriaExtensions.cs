using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Application.Extensions
{
    public static class CategoriaExtensions
    {
        public static Categoria ToService(this CategoriaResponseInfra infra)
        {
            return new Categoria
            {
                Nome = infra.Nome,
                Descricao = infra.Descricao,
                Tipo = infra.Tipo,
                IdCategoria = infra.IdCategoria,
            };
        }

        public static CategoriaResponseViewModel ToViewModel(this Categoria domain)
        {
            return new CategoriaResponseViewModel
            {
                Nome = domain.Nome,
                Descricao = domain.Descricao,
                Tipo = domain.Tipo,
                IdCategoria = domain.IdCategoria,
            };
        }

        public static CadastrarCategoriaRequestInfra ToInfra(this CadastrarCategoriaRequestViewModel viewModel)
        {
            return new CadastrarCategoriaRequestInfra
            {
                Nome = viewModel.Nome,
                Tipo = viewModel.Tipo,
                Descricao = viewModel.Descricao
            };
        }

        public static AtualizarCategoriaRequestInfra ToInfra(this AtualizarCategoriaRequestViewModel viewModel)
        {
            return new AtualizarCategoriaRequestInfra
            {
                Nome = viewModel.Nome,
                Tipo = viewModel.Tipo,
                Descricao = viewModel.Descricao
            };
        }
    }
}

