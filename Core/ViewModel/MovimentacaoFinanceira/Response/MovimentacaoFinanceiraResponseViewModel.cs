namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class MovimentacaoFinanceiraResponseViewModel
    {
        public string TipoMovimentacao { get; set; }
        public Decimal Valor { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string? Descricao { get; set; }
        public string? NumeroContaOrigem { get; set; }
        public string? NumeroContaDestino { get; set; }
    }
}
