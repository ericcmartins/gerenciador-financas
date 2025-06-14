namespace gerenciador.financas.API.ViewModel.Cliente
{
    public class ReceitaRequestViewModel
     {
        public decimal? Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataReceita { get; set; }
        public bool? Recorrente { get; set; }
        public int? Frequencia { get; set; }
    }

}
