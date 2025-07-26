using FluentValidation;
using gerenciador.financas.API.ViewModel.Cliente;


namespace gerenciador.financas.API.Validators
{
    public class CadastrarCategoriaRequestViewModelValidator : AbstractValidator<CadastrarCategoriaRequestViewModel>
    {
        public CadastrarCategoriaRequestViewModelValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome da categoria deve ser informado")
                .MaximumLength(20).WithMessage("O nome da categoria não pode exceder 50 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(20).WithMessage("A descrição não pode exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Nome));

            RuleFor(x => x.Tipo)
                .NotEmpty().WithMessage("O tipo da categoria deve ser informado")
                .Must(tipo => tipo == "Receita" || tipo == "Despesa")
                .WithMessage("O tipo deve ser 'Receita' ou 'Despesa'");
        }
    }
}

