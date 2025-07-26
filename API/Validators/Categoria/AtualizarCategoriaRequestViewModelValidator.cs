using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class AtualizarCategoriaRequestViewModelValidator : AbstractValidator<AtualizarCategoriaRequestViewModel>
    {
        public AtualizarCategoriaRequestViewModelValidator()
        {
            RuleFor(x => x.Nome)
                .MaximumLength(20).WithMessage("O nome da categoria não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Nome));

            RuleFor(x => x.Descricao)
                .MaximumLength(20).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Nome));

            RuleFor(x => x.Tipo)
                .Must(tipo => tipo == "Receita" || tipo == "Despesa")
                .WithMessage("O tipo deve ser 'Receita' ou 'Despesa'")
                .When(x => !string.IsNullOrEmpty(x.Tipo));
        }
    }
}

