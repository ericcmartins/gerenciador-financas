namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class ItemListaCompraRequestInfra
    {
        public int IdItem { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public string Prioridade { get; set; }
        public string Comprado { get; set; }
        public int IdLista { get; set; }
    }

}
