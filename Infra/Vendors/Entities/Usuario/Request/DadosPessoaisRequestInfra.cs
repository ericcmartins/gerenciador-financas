namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class DadosPessoaisRequestInfra
    {
        public string? Nome { get; set; }  
        public string? Email { get; set; }  
        public string? Senha { get; set; }
        public string RoleUsuario { get; set; } = "User";
        public DateTime? DataNascimento { get; set; }  
        public string? Telefone { get; set; }
    }

}
