namespace gerenciador.financas.Domain.Entities
{
    public class MetodoPagamento
    {
        public int IdMetodo { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal? Limite { get; set; }
        public string Tipo { get; set; }
        public int IdUsuario { get; set; }
        public int IdConta { get; set; }
    }
}