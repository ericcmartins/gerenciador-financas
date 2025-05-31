namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class ReceitaRequestInfra
    {
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
        public int IdConta {  get; set; }
        public int IdCategoria { get; set; }
    }

}
