using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Application.ViewModel.Cliente
{
    public class ReceitaPorContaResponseViewModel
    {
        public string Instituicao { get; set; }
        public string TipoConta { get; set; }
        public decimal TotalReceita { get; set; }
    }

}

