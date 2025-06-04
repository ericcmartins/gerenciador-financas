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
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
        public int IdUsuario { get; set; }
        public int IdConta { get; set; }
        public int IdCategoria { get; set; }
    }

}

