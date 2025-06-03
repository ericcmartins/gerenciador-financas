namespace gerenciador.financas.Domain.Entities
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int IdUsuario { get; set; }
    }
}