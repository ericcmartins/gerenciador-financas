namespace gerenciador.financas.Domain.Entities
{
    public class Conta
    {
        public string NumeroConta { get; set; }
        public string? Tipo { get; set; }
        public string? Instituicao { get; set; }
        public int IdConta { get; set; }
    }
}