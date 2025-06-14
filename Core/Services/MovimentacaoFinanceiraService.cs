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
            var responseInfra = await _movimentacaoFinanceiraRepository.GetMovimentacoesFinanceiras(idUsuario, periodo);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return null;

            var movimentacoesFinanceiras = responseInfra
                .Select(mf => mf.ToService())
                .Where(mf => tipoMovimentacao == null || mf.TipoMovimentacao == tipoMovimentacao)
                .ToList();

            return movimentacoesFinanceiras;
        }

        public async Task<bool> InsertTransferenciaEntreContas(MovimentacaoFinanceiraRequestViewModel movimentacaoFinanceiraRequest, int idUsuario, int idContaOrigem, int idContaDestino)
        {
            var resultado = await _movimentacaoFinanceiraRepository.InsertTransferenciaEntreContas(movimentacaoFinanceiraRequest.ToInfra(), idUsuario, idContaOrigem, idContaDestino);
            if (_movimentacaoFinanceiraRepository.HasNotifications)
                return false;

            return resultado;
        }
    }
}
