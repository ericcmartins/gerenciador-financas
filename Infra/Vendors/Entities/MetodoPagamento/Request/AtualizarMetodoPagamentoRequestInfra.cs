namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class AtualizarMetodoPagamentoRequestInfra
    {
        public string? Nome { get; set; }
        public int? IdTipoMetodo { get; set; }
        public Decimal? Limite { get; set; }
    }

}
