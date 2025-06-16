namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class MetaFinanceiraRequestViewModel
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal ValorAlvo { get; set; }
        public Decimal ValorAtual { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
    }

}
