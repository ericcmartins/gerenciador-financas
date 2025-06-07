namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class MovimentacaoFinanceiraRequestViewModel
     {
        public string TipoMovimentacao { get; set; }
        public Decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
    }

}
