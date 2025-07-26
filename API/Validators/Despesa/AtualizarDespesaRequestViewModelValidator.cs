using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class AtualizarDespesaRequestViewModelValidator : AbstractValidator<AtualizarDespesaRequestViewModel>
    {
        public AtualizarDespesaRequestViewModelValidator()
        {
            RuleFor(x => x.Valor)
                 .Must(valor => valor is null || valor > 0).WithMessage("O valor da despesa deve ser maior que 0");

            RuleFor(x => x.Descricao)
                .MaximumLength(50).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descricao));
        }
    }
}
