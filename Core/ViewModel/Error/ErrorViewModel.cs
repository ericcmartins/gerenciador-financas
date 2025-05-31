using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel
{
namespace gerenciador.financas.API.ViewModels
{
    public class ErrorViewModel
    {
        public int StatusCode { get; set; }
        public string Mensagem { get; set; }

        public ErrorViewModel(int status, string mensagem)
        {
            StatusCode = status;
            Mensagem = mensagem;
        }
    }
}

}
