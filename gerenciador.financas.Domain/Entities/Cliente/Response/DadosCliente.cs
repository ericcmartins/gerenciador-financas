namespace gerenciador.financas.Domain.Entities.Cliente
{
    public class DadosPessoais
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; }
    }
}