namespace gerenciador.financas.Domain.Entities
{
    public class ListaCompra
    {
        public int IdLista { get; set; }
        public string Titulo { get; set; }
        public DateTime Criacao { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
        public string Descricao { get; set; }
        public int IdUsuario { get; set; }
    }
}