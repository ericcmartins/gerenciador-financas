using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class MetaFinanceiraResponseInfra
    {
        public int? IdMetaFinanceira { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal? ValorAlvo { get; set; }
        public Decimal? ValorAtual { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public bool? Concluida { get; set; } 
        public int? IdUsuario { get; set; }
    }

}

