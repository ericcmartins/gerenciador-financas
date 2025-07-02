namespace gerenciador.financas.Domain.Entities
{
    public class Despesa
    {
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataDespesa { get; set; }
        public string? Conta { get; set; }
        public string? Categoria { get; set; }
        public string? MetodoPagamento { get; set; }
    }
}