namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class ReceitaResponseViewModel
    {
        public int IdReceita { get; set; }
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataReceita { get; set; }
        public string Categoria { get; set; }
        public string? Instituicao { get; set; }
        public string? TipoConta { get; set; }
    }
}
