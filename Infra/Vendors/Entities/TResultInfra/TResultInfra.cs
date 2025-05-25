using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Vendors.Entities
{
    public class TResultInfra<T>
    {
        public bool Sucesso { get; set; }
        public T ObjetoClasse { get; set; }
        public ErrosInfra TipoErro { get; set; }
        public string MensagemErro { get; set; }

        private TResultInfra(bool sucesso, T objetoClasse, ErrosInfra tipoErro, string mensagemErro)
        {
            Sucesso = sucesso;
            ObjetoClasse = objetoClasse;
            TipoErro = tipoErro;
            MensagemErro = mensagemErro;
        }

        public static TResultInfra<T> Ok(T objetoClasse)
        {
            return new TResultInfra<T>(true, objetoClasse, ErrosInfra.None, null);
        }

        public static TResultInfra<T> Fail(ErrosInfra tipoErro, string mensagemErro)
        {
            return new TResultInfra<T>(false, default, tipoErro, mensagemErro);
        }
    }
    public enum ErrosInfra
    {
        None,
        NotFound,
        DatabaseError
    }

}
