using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class CadastrarMetodoPagamentoRequestViewModelValidator : AbstractValidator<CadastrarMetodoPagamentoRequestViewModel>
    {
        public CadastrarMetodoPagamentoRequestViewModelValidator()
        {
            RuleFor(x => x.IdTipoMetodo)
                .InclusiveBetween(1, 5).WithMessage("O Id de tipo de conta deve estar entre 1 e 5.");

            RuleFor(x => x.Limite)
                .GreaterThanOrEqualTo(1).WithMessage("O limite deve ser maior que 0")
                .When(x => x.Limite.HasValue);

            RuleFor(x => x.Nome)
                .MaximumLength(20).WithMessage("O nome do método não pode exceder 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Nome));
        }
    }
}
