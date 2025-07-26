namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class AtualizarDespesaRequestViewModel
    {
        public decimal? Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataDespesa { get; set; }
    }

}
