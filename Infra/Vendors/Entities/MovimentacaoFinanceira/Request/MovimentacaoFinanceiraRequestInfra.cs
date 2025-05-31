namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class MovimentacaoFinanceiraRequestInfra
    {
        public string TipoMovimentacao { get; set; }
        public Decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
        public int IdConta { get; set; }
    }

}
