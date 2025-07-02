namespace gerenciador.financas.Infra.Auth
{
    public interface IAuthService
    {
        public string ComputeHash(string password);
        public string GenerateToken(string email, string role);
        public Task<string> Login(string email, string senha);
    }
}
