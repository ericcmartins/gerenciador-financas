namespace gerenciador.financas.Domain.Entities
{
    public class Usuario
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; }
    }
}