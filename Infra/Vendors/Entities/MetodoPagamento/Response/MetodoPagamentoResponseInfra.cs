using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class MetodoPagamentoResponseInfra
    {
        public int? IdMetodo { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public Decimal? Limite { get; set; }
        public string? Tipo { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdConta { get; set; }
    }

}

