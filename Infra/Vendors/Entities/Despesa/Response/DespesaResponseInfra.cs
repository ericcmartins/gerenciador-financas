using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class DespesaResponseInfra
    {
        public int IdDespesa { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime DataDespesa { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
        public int IdUsuario { get; set; }
        public int IdConta { get; set; }
        public int IdCategoria { get; set; }
        public int IdMetodoPagamento { get; set; }
    }

}

