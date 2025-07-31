namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class ContaResponseViewModel
    {
        public int IdConta { get; set; }
        public string? NumeroConta { get; set; }
        public string TipoConta { get; set; }
        public string Instituicao { get; set; }
        public int IdUsuario { get; set; }
    }
}
