namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class MovimentacaoFinanceiraResponseViewModel
    {
        public string TipoMovimentacao { get; set; }
        public Decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
    }
}
