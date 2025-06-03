namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class DadosPessoaisRequestViewModel
     {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public DateTime? DataNascimento { get; set; } //garantir que a data venha do front com dd/mm/yyyy
        public string? Telefone { get; set; }
    }

}
