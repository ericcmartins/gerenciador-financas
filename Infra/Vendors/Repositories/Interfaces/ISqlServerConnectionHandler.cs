using System.Data;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface ISqlServerConnectionHandler
    {
        IDbConnection CreateConnection();
    }
}