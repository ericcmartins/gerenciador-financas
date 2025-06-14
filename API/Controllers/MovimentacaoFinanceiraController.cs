using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using gerenciador.financas.Infra.Vendors;
using Core.ViewModel.gerenciador.financas.API.ViewModels;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class MovimentacaoFinanceiraController : Controller
    {
        private readonly IMovimentacaoFinanceiraService _movimentacaoFinanceiraService;
        private readonly NotificationPool _notificationPool;

        public MovimentacaoFinanceiraController(IMovimentacaoFinanceiraService movimentacaoFinanceiraService,
                                 NotificationPool notificationPool)
        {
            _movimentacaoFinanceiraService = movimentacaoFinanceiraService;
            _notificationPool = notificationPool;
        }

        [HttpGet("movimentacoes/cliente")]
        [ProducesResponseType(typeof(List<MovimentacaoFinanceiraResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterDespesasUsuario([Required] int idUsuario, int periodo, string tipoMovimentacao)
        {
            try
            {
                var response = await _movimentacaoFinanceiraService.GetMovimentacoesFinanceiras(idUsuario, periodo, tipoMovimentacao);
                if (_movimentacaoFinanceiraService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(mf => mf.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("movimentacao/cliente")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InserirDespesa([Required][FromBody] MovimentacaoFinanceiraRequestViewModel movimentacaoFinanceiraRequestViewModel,
                                                 [Required] int idUsuario,
                                                 [Required] int idContaOrigem,
                                                 [Required] int idContaDestino)

        {
            try
            {
                var response = await _movimentacaoFinanceiraService.InsertTransferenciaEntreContas(movimentacaoFinanceiraRequestViewModel, idUsuario, idContaOrigem, idContaDestino);
                if (_movimentacaoFinanceiraService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Created(string.Empty, "movimentação entre contas inserida com sucesso");
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}


