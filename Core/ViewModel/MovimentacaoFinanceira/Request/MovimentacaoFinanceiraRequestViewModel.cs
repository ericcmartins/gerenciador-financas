using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class MovimentacaoFinanceiraRequestViewModel
    {
        public string TipoMovimentacao => "Transferência";
        public decimal Valor {  get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string? Descricao { get; set; }
    }
}
