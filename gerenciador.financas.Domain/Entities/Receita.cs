namespace gerenciador.financas.Domain.Entities
{
    public class Receita
    {
        public decimal? Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataReceita { get; set; }
        public bool? Recorrente { get; set; }
        public int? Frequencia { get; set; }
        public string Conta { get; set; }
        public string Categoria { get; set; }
    }
}