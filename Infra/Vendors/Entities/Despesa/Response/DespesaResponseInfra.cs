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
        public string? Descricao { get; set; }
        public DateTime DataDespesa { get; set; }
        public int IdUsuario { get; set; }
        public string? Categoria { get; set; }
        public string? Instituicao { get; set; }
        public string? TipoConta { get; set; }
        public string MetodoPagamento { get; set; }
    }
}

