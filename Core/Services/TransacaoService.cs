using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public TransacaoService(ITransacaoRepository movimentacaoFinanceiraRepository,
                              NotificationPool notificationPool)
        {
            _transacaoRepository = movimentacaoFinanceiraRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<MovimentacaoFinanceira?>> GetMovimentacoesFinanceiras(int idUsuario, int periodo, string? tipoMovimentacao)
        {
            var responseInfra = await _transacaoRepository.GetMovimentacoesFinanceiras(idUsuario, periodo, tipoMovimentacao);
            if (_transacaoRepository.HasNotifications)
                return null;

            var movimentacoesFinanceiras = responseInfra
                .Select(mf => mf.ToService())
                .Where(mf => tipoMovimentacao == null || mf.TipoMovimentacao == tipoMovimentacao)
                .ToList();

            return movimentacoesFinanceiras;
        }

        public async Task<List<SaldoContas?>> GetSaldoPorConta(int idUsuario)
        {
            var responseInfra = await _transacaoRepository.GetSaldoPorConta(idUsuario);
            if (_transacaoRepository.HasNotifications)
                return null;

            var responseService = responseInfra
                .Select(sc => sc.ToService())
                .ToList();

            return responseService;
        }

        public async Task<List<SaldoTotalContas?>> GetSaldoTotalContas(int idUsuario)
        {
            var responseInfra = await _transacaoRepository.GetSaldoTotalContas(idUsuario);
            if (_transacaoRepository.HasNotifications)
                return null;

            var responseService = responseInfra
                .Select(sc => sc.ToService())
                .ToList();

            return responseService;
        }

        public async Task<bool> InsertTransferenciaEntreContas(CadastrarTransacaoRequestViewModel transacaoRequest, int idUsuario, int idContaOrigem, int idContaDestino)
        {
            var resultado = await _transacaoRepository.InsertTransferenciaEntreContas(transacaoRequest.ToInfra(), idUsuario, idContaOrigem, idContaDestino);
            if (_transacaoRepository.HasNotifications)
                return false;

            return resultado;
        }
        public async Task<bool> UpdateMovimentacaoFinanceira(AtualizarTransacaoRequestViewModel transacaoRequest, int idUsuario, int idContaOrigem, int idContaDestino, int idMovimentacaoFinanceira)
        {
            var resultado = await _transacaoRepository.UpdateMovimentacaoFinanceira(transacaoRequest.ToInfra(), idUsuario, idContaOrigem, idContaDestino, idMovimentacaoFinanceira);
            if (_transacaoRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> DeleteMovimentacaoFinanceira(int idUsuario, int idMovimentacaoFinanceira)
        {
            var resultado = await _transacaoRepository.DeleteMovimentacaoFinanceira(idUsuario, idMovimentacaoFinanceira);
            if (_transacaoRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
