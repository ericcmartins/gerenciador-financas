using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class LoginResponseInfra
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; } 
        public string Nome { get; set; }
    }

}

