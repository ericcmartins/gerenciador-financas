namespace gerenciador.financas.Domain.Entities
{
    public class MetaFinanceira
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal ValorAlvo { get; set; }
        public Decimal ValorAtual { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public bool Concluida { get; set; }
        public int IdMetaFinanceira { get; set; }
    }
}