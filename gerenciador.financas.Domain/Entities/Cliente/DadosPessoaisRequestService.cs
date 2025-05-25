namespace gerenciador.financas.Domain.Entities.Cliente
{
    public class DadosPessoaisRequestService
     {
        public string nome { get; set; }
        public string cpf { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public DateTime data_nascimento { get; set; }
        public string telefone { get; set; }
    }

}
