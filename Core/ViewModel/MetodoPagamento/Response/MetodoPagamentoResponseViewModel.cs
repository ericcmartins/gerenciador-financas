namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class MetodoPagamentoResponseViewModel
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal? Limite { get; set; }
        public string? Tipo { get; set; }
        public int IdMetodo { get; set; }
        public string? NumeroConta { get; set; }
        public int IdConta { get; set; }
    }
}
