using gerenciador.financas.Infra.Vendors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Repositories
{
    public interface IPagamentoRepository : INotifiable
    {
        public Task<List<MetodoPagamentoResponseInfra?>> GetMetodosPagamentoUsuario(int idUsuario);
        public Task<bool> InsertMetodoPagamento(CadastrarMetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario, int idConta);
        public Task<bool> UpdateMetodoPagamento(AtualizarMetodoPagamentoRequestInfra metodoPagamentoRequest, int idUsuario, int idConta, int idMetodoPagamento);
        public Task<bool> DeleteMetodoPagamento(int idUsuario, int idMetodoPagamento);
    }
}
