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
                Descricao = infra.Descricao
            };
        }

        public static CategoriaResponseViewModel ToViewModel(this Categoria domain)
        {
            return new CategoriaResponseViewModel
            {
                Nome = domain.Nome,
                Descricao = domain.Descricao
            };
        }

        public static CategoriaRequestInfra ToInfra(this CategoriaRequestViewModel viewModel)
        {
            return new CategoriaRequestInfra
            {
                Nome = viewModel.Nome,
                Descricao = viewModel.Descricao

            };
        }
    }
}

