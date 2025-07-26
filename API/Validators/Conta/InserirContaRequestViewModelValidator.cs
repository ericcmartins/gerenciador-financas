using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class InserirContaRequestViewModelValidator : AbstractValidator<InserirContaRequestViewModel>
    {
        public InserirContaRequestViewModelValidator()
        {
            RuleFor(x => x.IdTipoConta)
                .NotEmpty().WithMessage("O Id de tipo de conta é obrigatório.")
                .InclusiveBetween(1, 5).WithMessage("O Id de tipo de conta deve estar entre 1 e 5.");

            RuleFor(x => x.Instituicao)
                .NotEmpty().WithMessage("O Id de tipo de conta é obrigatório.")
                .MaximumLength(30).WithMessage("O telefone não pode exceder 20 caracteres.");

            RuleFor(x => x.NumeroConta)
                .MaximumLength(20).WithMessage("O número da conta não pode exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.NumeroConta)); 
        }
    }
}
