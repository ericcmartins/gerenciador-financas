namespace gerenciador.financas.Domain.Entities
{
    public class MovimentacaoFinanceira
    {
        public string TipoMovimentacao { get; set; }
        public Decimal Valor { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string? Descricao { get; set; }
        public string? NumeroContaOrigem { get; set; }
        public string? NumeroContaDestino { get; set; }
        public int IdMovimentacao { get; set; }
    }
}