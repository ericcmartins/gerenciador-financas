using System;
using System.Net;

namespace gerenciador.financas.Domain.Entities
{
    public class TResultService<T>
    {
        public T? ObjetoClasse { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public TResultService(T objetoClasse, HttpStatusCode statusCode)
        {
            ObjetoClasse = objetoClasse;
            StatusCode = statusCode;
        }
    }
}