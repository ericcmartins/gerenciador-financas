namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class AtualizarDadosCadastraisRequestViewModel
     {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Telefone { get; set; }
    }

}
