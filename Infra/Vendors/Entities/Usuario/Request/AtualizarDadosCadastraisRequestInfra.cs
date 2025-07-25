namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class AtualizarDadosCadastraisRequestInfra
    {
        public string? Nome { get; set; }  
        public string? Email { get; set; }
        public string? SenhaHash { get; set; }
        public DateTime? DataNascimento { get; set; }  
        public string? Telefone { get; set; }
    }

}
