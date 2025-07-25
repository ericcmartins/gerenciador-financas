using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class AtualizarSenhaRequestViewModelValidator : AbstractValidator<AtualizarSenhaRequestViewModel>
    {
        public AtualizarSenhaRequestViewModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail não informado") 
                .MaximumLength(50).WithMessage("O e-mail não pode exceder 50 caracteres.")
                .EmailAddress().WithMessage("O formato do e-mail é inválido.");

            RuleFor(x => x.NovaSenha)
                .MaximumLength(20).WithMessage("A senha não pode exceder 50 caracteres.")
                .NotEmpty().WithMessage("Senha não informada");

            RuleFor(x => x.Telefone)
                .MaximumLength(20).WithMessage("O telefone não pode exceder 20 caracteres.")
                .NotEmpty().WithMessage("Telefone não informado");
        }
    }
}
