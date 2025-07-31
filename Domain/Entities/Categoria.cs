namespace gerenciador.financas.Domain.Entities
{
    public class Categoria
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string Tipo { get; set; }
        public int IdCategoria { get; set; }
    }
}