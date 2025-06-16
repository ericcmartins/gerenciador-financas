using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class ReceitaResponseInfra
    {
        public int IdReceita {  get; set; } 
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataReceita { get; set; }
        public bool Recorrente { get; set; }
        public int? Frequencia { get; set; }
        public int IdUsuario { get; set; }
        public string Conta { get; set; }
        public string? Categoria { get; set; }
    }

}

