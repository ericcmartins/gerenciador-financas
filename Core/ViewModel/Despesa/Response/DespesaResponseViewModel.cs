namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class DespesaResponseViewModel
    {
        public decimal? Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataDespesa { get; set; }
        public bool? Recorrente { get; set; }
        public int? Frequencia { get; set; }
    }
}
