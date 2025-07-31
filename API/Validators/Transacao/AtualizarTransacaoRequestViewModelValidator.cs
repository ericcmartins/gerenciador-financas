using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class AtualizarTransacaoRequestViewModelValidator : AbstractValidator<AtualizarTransacaoRequestViewModel>
    {
        public AtualizarTransacaoRequestViewModelValidator()
        {
            RuleFor(x => x.Valor)
                 .Must(valor => valor is null || valor > 0).WithMessage("O valor da receita deve ser maior que 0");

            RuleFor(x => x.Descricao)
                .MaximumLength(50).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descricao));

            RuleFor(x => x.DataMovimentacao)
                .Must(data => data == null || data >= DateTime.Today.AddYears(-60)).WithMessage("A data de início é muito antiga.");
        }
    }
}
