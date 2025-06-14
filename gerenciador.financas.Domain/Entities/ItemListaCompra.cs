namespace gerenciador.financas.Domain.Entities
{
    public class ItemListaCompra
    {
        public int? IdItem { get; set; }
        public string? Descricao { get; set; }
        public int? Quantidade { get; set; }
        public string? Prioridade { get; set; }
        public string? Comprado { get; set; }
        public int? IdLista { get; set; }
    }
}