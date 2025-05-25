using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Entities
{
    public class DadosPessoaisResponseInfra
    {
        public int id_usuario { get; set; }
        public string nome { get; set; }
        public string cpf { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public DateTime data_nascimento { get; set; }
        public string telefone { get; set; }
    }
        
}

