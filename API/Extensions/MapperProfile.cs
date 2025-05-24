using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Core.Entities;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Extensions
{
    public static class MapperProfile
    {
        public static DadosPessoaisRequestService ToServiceRequest(this DadosPessoaisRequestViewModel viewModel)
        {
            return new DadosPessoaisRequestService
            {
                cpf = viewModel.cpf,
                nome = viewModel.nome,
                telefone = viewModel.telefone,
                dataNascimento = viewModel.dataNascimento
            };
        }

        public static DadosPessoaisRequestInfra ToInfraRequest(this DadosPessoaisRequestService viewModel)
        {
            return new DadosPessoaisRequestInfra
            {
                cpf = viewModel.cpf,
                nome = viewModel.nome,
                telefone = viewModel.telefone,
                dataNascimento = viewModel.dataNascimento
            };
        }
    }
}