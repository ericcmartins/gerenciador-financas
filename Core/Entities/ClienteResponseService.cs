namespace gerenciador.financas.Core.Entities
{
    public class ClienteResponseService
    {
        public int id_usuario { get; set; }
        public string nome { get; set; }
        public string cpf { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public DateTime data_nascimento { get; set; }
        public string telefone { get; set;}
    }
}