namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class CadastrarReceitaRequestInfra
    {
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataReceita { get; set; }
    }

}
