using Core.ViewModel;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.Extensions
{
    public static class MapperProfile
    {
        //    public static DadosPessoaisRequestService DadosPessoaisViewlModelToService(this DadosPessoaisRequestViewModel viewModel)
        //    {
        //        return new DadosPessoaisRequestService
        //        {
        //            cpf = viewModel.cpf,
        //            nome = viewModel.nome,
        //            email = viewModel.email,
        //            senha = viewModel.senha,
        //            telefone = viewModel.telefone,
        //            data_nascimento = viewModel.data_nascimento
        //        };
        //    }

        //    public static DadosPessoaisRequestInfra DadosPessoaiServiceToInfra(this DadosPessoaisRequestService service)
        //    {
        //        return new DadosPessoaisRequestInfra
        //        {
        //            cpf = service.cpf,
        //            nome = service.nome,
        //            email = service.email,
        //            senha = service.senha,
        //            telefone = service.telefone,
        //            data_nascimento = service.data_nascimento
        //        };
        //    }

        //    public static DadosPessoaisResponseService DadosPessoaisResponseInfraToService(this DadosPessoaisResponseInfra infra)
        //    {
        //        return new DadosPessoaisResponseService
        //        {
        //            cpf = infra.cpf,
        //            nome = infra.nome,
        //            email = infra.email,
        //            senha = infra.senha,
        //            telefone = infra.telefone,
        //            data_nascimento = infra.data_nascimento
        //        };
        //    }
        //    public static DadosPessoaisResponseViewModel DadosPessoaisResponseServiceToViewModel(this DadosPessoaisResponseService service)
        //    {
        //        return new DadosPessoaisResponseViewModel
        //        {
        //            cpf = service.cpf,
        //            nome = service.nome,
        //            telefone = service.telefone,
        //            data_nascimento = service.data_nascimento
        //        };
        //    }

        //    public static TResultViewModel<T> TResultServiceToViewModel<T>(this TResultService<T> service)
        //    {
        //        return new TResultViewModel<T>(service.ObjetoClasse, service.StatusCode);
        //    }
        //}
    }
}