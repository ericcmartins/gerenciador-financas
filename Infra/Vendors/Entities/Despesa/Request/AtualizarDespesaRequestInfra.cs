namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class AtualizarDespesaRequestInfra
    {
        public decimal? Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataDespesa { get; set; }
    }

}
