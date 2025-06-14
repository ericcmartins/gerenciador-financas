using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class MovimentacaoFinanceiraResponseInfra
    {
        public int? IdMovimentacao { get; set; }
        public string? TipoMovimentacao { get; set; }
        public Decimal? Valor { get; set; }
        public DateTime? DataMovimentacao { get; set; }
        public string? Descricao { get; set; }
        public string? NumeroContaOrigem { get; set; }
        public string? NumeroContaDestino { get; set; }
        public int? IdReceita { get; set; }
        public int? IdDespesa { get; set; }
    }

}

