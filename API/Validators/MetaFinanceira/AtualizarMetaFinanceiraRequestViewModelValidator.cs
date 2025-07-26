using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class AtualizarMetaFinanceiraRequestViewModelValidator : AbstractValidator<AtualizarMetaFinanceiraRequestViewModel>
    {
        public AtualizarMetaFinanceiraRequestViewModelValidator()
        {
            RuleFor(x => x.Nome)
                .MaximumLength(50).WithMessage("O nome da meta não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Nome));

            RuleFor(x => x.Descricao)
                .MaximumLength(20).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Nome));

            RuleFor(x => x.ValorAlvo)
                .Must(valor => valor is null || valor > 0).WithMessage("O valor alvo deve ser maior que 0");

            RuleFor(x => x.ValorAtual)
                .Must(valor => valor is null || valor >= 0).WithMessage("O valor alvo não pode ser menor que 0");

            RuleFor(x => x.DataInicio)
                .Must(data => data == null || data >= DateTime.Today.AddYears(-60))
                .WithMessage("A data de início é muito antiga.");
        }
    }
}

