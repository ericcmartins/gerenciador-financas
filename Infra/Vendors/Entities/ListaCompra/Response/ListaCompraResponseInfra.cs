using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class ListaCompraResponseInfra
    {
        public int IdLista { get; set; }
        public string Titulo { get; set; }
        public DateTime Criacao { get; set; }
        public bool Recorrente { get; set; }
        public int Frequencia { get; set; }
        public string Descricao { get; set; }
        public int IdUsuario { get; set; }
    }
}

