using System.Data;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IDbConnectionHandler
    {
        IDbConnection CreateConnection();
    }
}