namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class DespesaRequestViewModel
     {
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
    }

}
