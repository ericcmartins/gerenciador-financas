namespace gerenciador.financas.Domain.Entities
{
    public class Receita
    {
        public int IdReceita {  get; set; } 
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataReceita { get; set; }
        public int IdUsuario { get; set; }
        public string Categoria { get; set; }
        public string? Instituicao { get; set; }
        public string? TipoConta { get; set; }
    }
}