namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class ContaRequestViewModel
     {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Telefone { get; set; }
    }

}
