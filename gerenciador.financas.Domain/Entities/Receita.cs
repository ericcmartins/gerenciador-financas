namespace gerenciador.financas.Domain.Entities
{
    public class Receita
    {
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
    }
}