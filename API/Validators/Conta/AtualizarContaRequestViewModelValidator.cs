using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class AtualizarContaRequestViewModelValidator : AbstractValidator<AtualizarContaRequestViewModel>
    {
        public AtualizarContaRequestViewModelValidator()
        {
            RuleFor(x => x.IdTipoConta)
                .InclusiveBetween(1, 5).WithMessage("O Id de tipo de conta deve estar entre 1 e 5.")
                .When(x => x.IdTipoConta.HasValue);

            RuleFor(x => x.Instituicao)
                .MaximumLength(30).WithMessage("O telefone não pode exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Instituicao));

            RuleFor(x => x.NumeroConta)
                .MaximumLength(20).WithMessage("O número da conta não pode exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.NumeroConta)); 
        }
    }
}
