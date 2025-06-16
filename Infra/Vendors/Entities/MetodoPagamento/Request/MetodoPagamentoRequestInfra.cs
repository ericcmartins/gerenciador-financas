namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class MetodoPagamentoRequestInfra
    {
        public string Nome {  get; set; }
        public string? Descricao { get; set; }
        public Decimal? Limite { get; set; }
        public string? Tipo { get; set; }
    }

}
