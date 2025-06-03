using System.Net;

namespace gerenciador.financas.Infra.Vendors
{
    public class Notification
    {
        public int StatusCode { get; }
        public string Mensagem { get; }
        public Notification(int statusCode, string mensagem)
        {
            StatusCode = statusCode;
            Mensagem = mensagem;
        }
    }
}
