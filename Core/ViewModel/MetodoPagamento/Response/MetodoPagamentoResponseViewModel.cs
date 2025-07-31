namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class MetodoPagamentoResponseViewModel
    {
        public int IdMetodo { get; set; }
        public string? Nome { get; set; }
        public string TipoMetodoPagamento { get; set; }
        public int IdTipoMetodo { get; set; }
        public Decimal? Limite { get; set; }
        public int IdUsuario { get; set; }
        public string? NumeroConta { get; set; }
        public int? IdConta { get; set; }
    }
}
