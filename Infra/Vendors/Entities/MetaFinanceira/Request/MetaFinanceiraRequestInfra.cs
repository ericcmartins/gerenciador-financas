namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class MetaFinanceiraRequestInfra
    {
        public string Descricao { get; set; }
        public Decimal ValorAlvo { get; set; }
        public Decimal ValorAtual { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public bool Concluida { get; set; }
    }

}
