using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class SaldoPorContaResponseViewModel
    {
        public string NumeroConta { get; set; } 
        public string? Instituicao { get; set; }
        public string? Tipo { get; set; }
        public decimal SaldoConta { get; set; }
    }

}
