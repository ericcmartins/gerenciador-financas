namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class LoginResponseViewModel
    {
        public string Token { get; set; }
        public DateTime Expiracao { get; set; }
        public string Tipo { get; set; } = "Bearer";
        public int IdUsuario { get; set; }
    }
}

