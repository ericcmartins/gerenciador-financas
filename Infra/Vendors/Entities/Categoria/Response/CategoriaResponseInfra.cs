using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class CategoriaResponseInfra
    {
        public int IdCategoria { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string Tipo { get; set; }
        public int? IdUsuario { get; set; }
    }
}

