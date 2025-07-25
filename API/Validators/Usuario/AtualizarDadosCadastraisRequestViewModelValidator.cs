using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class AtualizarDadosCadastraisRequestViewModelValidator : AbstractValidator<AtualizarDadosCadastraisRequestViewModel>
    {
        public AtualizarDadosCadastraisRequestViewModelValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode exceder 50 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.") 
                .MaximumLength(50).WithMessage("O e-mail não pode exceder 50 caracteres.")
                .EmailAddress().WithMessage("O formato do e-mail é inválido.");

            RuleFor(x => x.DataNascimento)
                .NotNull().WithMessage("A data de nascimento é obrigatória.")
                .Must(data => data <= DateTime.Today.AddYears(-12))
                .WithMessage("O usuário deve ser maior de 12 anos.");

            RuleFor(x => x.Telefone)
                .MaximumLength(20).WithMessage("O telefone não pode exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Telefone)); 
        }
    }
}
