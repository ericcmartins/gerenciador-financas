using System.Net;

namespace gerenciador.financas.Infra.Vendors.Notification
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
