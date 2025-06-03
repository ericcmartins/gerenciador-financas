namespace gerenciador.financas.Domain.Entities
{
    public class Despesa
    {
        public int IdDespesa { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
        public int IdUsuario { get; set; }
        public int IdConta { get; set; }
        public int IdCategoria { get; set; }
        public int IdMetodoPagamento { get; set; }
    }
}