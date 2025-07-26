namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class CadastrarMetodoPagamentoRequestInfra
    {
        public string? Nome { get; set; }
        public int IdTipoMetodo { get; set; }
        public Decimal? Limite { get; set; }
    }

}
