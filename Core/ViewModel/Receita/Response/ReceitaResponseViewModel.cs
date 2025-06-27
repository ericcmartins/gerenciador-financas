namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class ReceitaResponseViewModel
    {
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataReceita { get; set; }
        public string Conta { get; set; }
        public string? Categoria { get; set; }
    }
}
