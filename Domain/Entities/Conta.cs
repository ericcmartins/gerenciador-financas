namespace gerenciador.financas.Domain.Entities
{
    public class Conta
    {
        public int IdConta { get; set; }
        public string? NumeroConta { get; set; }
        public string TipoConta { get; set; }
        public string Instituicao { get; set; }
        public int IdUsuario { get; set; }
    }
}