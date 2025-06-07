namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class MetodoPagamentoRequestViewModel
     {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal? Limite { get; set; }
        public string Tipo { get; set; }
    }

}
