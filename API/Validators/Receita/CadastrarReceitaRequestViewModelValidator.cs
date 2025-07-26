using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class CadastrarReceitaRequestViewModelValidator : AbstractValidator<CadastrarReceitaRequestViewModel>
    {
        public CadastrarReceitaRequestViewModelValidator()
        {
            RuleFor(x => x.Valor)
                .NotEmpty().WithMessage("O valor da receita deve ser informado")
                .Must(valor => valor > 0).WithMessage("O valor da receita deve ser maior que 0");

            RuleFor(x => x.Descricao)
                .MaximumLength(50).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descricao));
        }
    }
}

