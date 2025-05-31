using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class ItemListaCompraResponseInfra
    {
        public int IdItem { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public string Prioridade { get; set; }
        public string Comprado { get; set; }
        public int IdLista { get; set; }
    }
}

