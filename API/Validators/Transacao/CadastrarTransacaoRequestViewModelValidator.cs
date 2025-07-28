using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Infra.Vendors.Entities;


namespace gerenciador.financas.API.Validators
{
    public class CadastrarTransacaoRequestViewModelValidator : AbstractValidator<CadastrarTransacaoRequestInfra>
    {
        public CadastrarTransacaoRequestViewModelValidator()
        {
            RuleFor(x => x.Valor)
                .NotEmpty().WithMessage("O valor da receita deve ser informado")
                .Must(valor => valor > 0).WithMessage("O valor da transação deve ser maior que 0");

            RuleFor(x => x.Descricao)
                .MaximumLength(50).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descricao));

            RuleFor(x => x.DataMovimentacao)
                .NotNull().WithMessage("A data da movimentação é obrigatória.")
                .Must(data => data == null || data >= DateTime.Today.AddYears(-60)).WithMessage("A data da transação é muito antiga.");
        }
    }
}

