using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class LoginRequestViewModelValidator : AbstractValidator<LoginRequestViewModel>
    {
        public LoginRequestViewModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail não informado") 
                .MaximumLength(50).WithMessage("O e-mail não pode exceder 50 caracteres.")
                .EmailAddress().WithMessage("O formato do e-mail é inválido.");

            RuleFor(x => x.Senha)
                .MaximumLength(20).WithMessage("A senha não pode exceder 50 caracteres.")
                .NotEmpty().WithMessage("Senha não informada");
        }
    }
}
