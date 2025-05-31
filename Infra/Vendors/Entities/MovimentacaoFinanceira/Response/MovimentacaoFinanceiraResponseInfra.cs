using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class MovimentacaoFinanceiraResponseInfra
    {
        public int IdMovimentacao { get; set; }
        public string TipoMovimentacao { get; set; }
        public Decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
        public int IdConta { get; set; }
        public int IdUsuario { get; set; }
    }

}

