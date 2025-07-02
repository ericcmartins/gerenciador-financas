using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Domain.Entities
{
    public class DespesaMetodoPagamento
    {
        public string MetodoPagamento { get; set; }
        public decimal TotalDespesa { get; set; }
    }

}

