using System;
using System.Net;

namespace Core.ViewModel;

public class TResultViewModel<T>
{
    public T? ObjetoClasse { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public TResultViewModel(T objetoClasse, HttpStatusCode statusCode)
    {
        ObjetoClasse = objetoClasse;
        StatusCode = statusCode;
    }
}