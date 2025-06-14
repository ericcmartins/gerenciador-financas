namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class DespesaRequestInfra
    {
        public decimal? Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataDespesa { get; set; }
        public bool? Recorrente { get; set; }
        public int? Frequencia { get; set; }
    }

}
