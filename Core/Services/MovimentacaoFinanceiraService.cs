using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Entities;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Entities;
using gerenciador.financas.Infra.Vendors.Repositories;

namespace gerenciador.financas.Application.Services
{
    public class MovimentacaoFinanceiraService : IMovimentacaoFinanceiraService
    {
        private readonly IMovimentacaoFinanceiraRepository _movimentacaoFinanceiraRepository;
        private readonly NotificationPool _notificationPool;
        public bool HasNotifications => _notificationPool.HasNotications;
        public IReadOnlyCollection<Notification> Notifications => _notificationPool.Notifications;
        public MovimentacaoFinanceiraService(IMovimentacaoFinanceiraRepository movimentacaoFinanceiraRepository,
                              NotificationPool notificationPool)
        {
            _movimentacaoFinanceiraRepository = movimentacaoFinanceiraRepository;
            _notificationPool = notificationPool;
        }

        public async Task<List<MovimentacaoFinanceira?>> GetMovimentacoesFinanceiras(int idUsuario, int? periodo, string? tipoMovimentacao)
        {
            var responseInfra = await _movimentacaoFinanceiraRepository.GetMovimentacoesFinanceiras(idUsuario, periodo, tipoMovimentacao);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return null;

            var movimentacoesFinanceiras = responseInfra
                .Select(mf => mf.ToService())
                .Where(mf => tipoMovimentacao == null || mf.TipoMovimentacao == tipoMovimentacao)
                .ToList();

            return movimentacoesFinanceiras;
        }

        public async Task<List<SaldoContas?>> GetSaldoPorConta(int idUsuario)
        {
            var responseInfra = await _movimentacaoFinanceiraRepository.GetSaldoPorConta(idUsuario);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return null;

            var responseService = responseInfra
                .Select(sc => sc.ToService())
                .ToList();

            return responseService;
        }

        public async Task<List<SaldoTotalContas?>> GetSaldoTotalContas(int idUsuario)
        {
            var responseInfra = await _movimentacaoFinanceiraRepository.GetSaldoTotalContas(idUsuario);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return null;

            var responseService = responseInfra
                .Select(sc => sc.ToService())
                .ToList();

            return responseService;
        }

        public async Task<bool> InsertTransferenciaEntreContas(MovimentacaoFinanceiraRequestViewModel movimentacaoFinanceiraRequest, int idUsuario, int idContaOrigem, int idContaDestino)
        {
            var resultado = await _movimentacaoFinanceiraRepository.InsertTransferenciaEntreContas(movimentacaoFinanceiraRequest.ToInfra(), idUsuario, idContaOrigem, idContaDestino);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return false;

            return resultado;
        }
        public async Task<bool> UpdateMovimentacaoFinanceira(MovimentacaoFinanceiraRequestViewModel movimentacaoFinanceiraRequest, int idUsuario, int idContaOrigem, int idContaDestino, int idMovimentacaoFinanceira)
        {
            var resultado = await _movimentacaoFinanceiraRepository.UpdateMovimentacaoFinanceira(movimentacaoFinanceiraRequest.ToInfra(), idUsuario, idContaOrigem, idContaDestino, idMovimentacaoFinanceira);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return false;

            return resultado;
        }

        public async Task<bool> DeleteMovimentacaoFinanceira(int idUsuario, int idMovimentacaoFinanceira)
        {
            var resultado = await _movimentacaoFinanceiraRepository.DeleteMovimentacaoFinanceira(idUsuario, idMovimentacaoFinanceira);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
