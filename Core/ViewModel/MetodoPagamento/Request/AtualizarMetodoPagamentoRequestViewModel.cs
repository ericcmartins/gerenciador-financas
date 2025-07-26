namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class AtualizarMetodoPagamentoRequestViewModel
     {
        public string? Nome { get; set; }
        public int? IdTipoMetodo { get; set; }
        public Decimal? Limite { get; set; }
    }

}
