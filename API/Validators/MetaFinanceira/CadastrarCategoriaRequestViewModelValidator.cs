using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class CadastrarMetaFinanceiraRequestViewModelValidator : AbstractValidator<CadastrarMetaFinanceiraRequestViewModel>
    {
        public CadastrarMetaFinanceiraRequestViewModelValidator()
        {
            RuleFor(x => x.Nome)
                .NotNull().WithMessage("O nome é obrigatório.")
                .MaximumLength(50).WithMessage("O nome da meta não pode exceder 50 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(20).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Nome));

            RuleFor(x => x.ValorAlvo)
                .NotNull().WithMessage("O valor alvo é obrigatório.")
                .Must(valor => valor > 0).WithMessage("O valor alvo deve ser maior que 0");

            RuleFor(x => x.ValorAtual)
                .NotNull().WithMessage("O valor atual é obrigatório.")
                .Must(valor => valor >= 0).WithMessage("O valor alvo não pode ser menor que 0");

            RuleFor(x => x.DataInicio)
                .NotNull().WithMessage("A data de início é obrigatória.")
                .Must(data => data == null || data >= DateTime.Today.AddYears(-60)).WithMessage("A data de início é muito antiga.");

            RuleFor(x => x.DataLimite)
                .NotNull().WithMessage("A data limite é obrigatória.")
                .GreaterThan(x => x.DataInicio).WithMessage("A data limite deve ser maior que a data de início.");

        }
    }
}

