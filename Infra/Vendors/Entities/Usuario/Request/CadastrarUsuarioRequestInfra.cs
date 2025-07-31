namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class CadastrarUsuarioRequestInfra
    {
        public string Nome { get; set; }  
        public string Email { get; set; }  
        public string SenhaHash { get; set; }
        public string RoleUsuario { get; set; } = "Usuario";
        public DateTime DataNascimento { get; set; }  
        public string Telefone { get; set; }
    }

}
