namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class MetaFinanceiraResponseViewModel
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal? ValorAlvo { get; set; }
        public Decimal? ValorAtual { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public bool? Concluida { get; set; }
    }
}
