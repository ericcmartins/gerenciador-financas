using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IMetodoPagamentoRepository
    {
        public Task<IEnumerable<MetodoPagamentoResponseInfra?>> GetMetodosPagamento(int idUsuario);
        public Task<bool> InsertMetodoPagamento(MetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario);
        public Task<bool> UpdateMetodoPagamento(MetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario);
        public Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento);
    }
}
