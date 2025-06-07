namespace gerenciador.financas.Domain.Entities
{
    public class MovimentacaoFinanceira
    {
        public string TipoMovimentacao { get; set; }
        public Decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
    }
}