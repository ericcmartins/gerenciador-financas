using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using gerenciador.financas.Infra.Vendors;
using Core.ViewModel.gerenciador.financas.API.ViewModels;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using gerenciador.financas.Infra.Vendors.Entities;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class TransacaoController : Controller
    {
        private readonly ITransacaoService _transacaoService;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<TransacaoController> _logger;

        public TransacaoController(ITransacaoService movimentacaoFinanceiraService,
                                   NotificationPool notificationPool,
                                   ILogger<TransacaoController> logger)
        {
            _transacaoService = movimentacaoFinanceiraService;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        [HttpGet("usuario/{idUsuario}/transacoes")]
        [ProducesResponseType(typeof(List<MovimentacaoFinanceiraResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransacoesUsuario([Required][FromRoute] int idUsuario, int periodo, string? tipoMovimentacao)
        {
            try
            {
                var response = await _transacaoService.GetMovimentacoesFinanceiras(idUsuario, periodo, tipoMovimentacao);
                if (_transacaoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar transa��es do usu�rio {IdUsuario} para o per�odo {Periodo}: Status Code - {StatusCode}, {Mensagem}", idUsuario, periodo, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(mf => mf.ToViewModel())
                    .ToList();

                _logger.LogInformation("Transa��es do usu�rio {IdUsuario} para o per�odo {Periodo} recuperadas com sucesso - Status Code - {StatusCode}", idUsuario, periodo, StatusCodes.Status200OK);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar transa��es do usu�rio {IdUsuario} para o per�odo {Periodo}", idUsuario, periodo);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }


        [HttpGet("usuario/{idUsuario}/saldo/contas")]
        [ProducesResponseType(typeof(List<SaldoPorContaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaldosPorConta([Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _transacaoService.GetSaldoPorConta(idUsuario);
                if (_transacaoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar saldos por conta do usu�rio {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(sc => sc.ToViewModel())
                    .ToList();

                _logger.LogInformation("Saldos por conta do usu�rio {IdUsuario} recuperados com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status200OK);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar saldos por conta do usu�rio {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpGet("usuario/{idUsuario}/saldo/total")]
        [ProducesResponseType(typeof(List<SaldoTotalUsuarioResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaldoTotalUsuario([Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _transacaoService.GetSaldoTotalContas(idUsuario);
                if (_transacaoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar saldo total do usu�rio {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(sc => sc.ToViewModel())
                    .ToList();

                _logger.LogInformation("Saldo total do usu�rio {IdUsuario} recuperado com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status200OK);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar saldo total do usu�rio {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/{idUsuario}/transacao-contas")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertMovimentacaoFinanceira([Required][FromBody] CadastrarTransacaoRequestViewModel transacaoRequestViewModel,
                                                                 [Required][FromRoute] int idUsuario,
                                                                 [Required] int idContaOrigem,
                                                                 [Required] int idContaDestino)

        {
            try
            {
                var response = await _transacaoService.InsertTransferenciaEntreContas(transacaoRequestViewModel, idUsuario, idContaOrigem, idContaDestino);
                if (_transacaoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao inserir transfer�ncia entre contas para o usu�rio {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Transfer�ncia entre contas para o usu�rio {IdUsuario} inserida com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status201Created);
                return Created(string.Empty, "movimenta��o entre contas inserida com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao inserir transfer�ncia entre contas para o usu�rio {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{idUsuario}/transacao-contas/{idTransacao}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMovimentacaoFinanceira([Required][FromBody] AtualizarTransacaoRequestViewModel transacaoRequestViewModel,
                                                                 [Required][FromRoute] int idUsuario,
                                                                 [Required][FromRoute] int idTransacao,
                                                                 [Required] int idContaOrigem,
                                                                 [Required] int idContaDestino)
        {
            try
            {
                var response = await _transacaoService.UpdateMovimentacaoFinanceira(transacaoRequestViewModel, idUsuario, idContaOrigem, idContaDestino, idTransacao);
                if (_transacaoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao atualizar transa��o {IdTransacao} para o usu�rio {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idTransacao, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Transa��o {IdTransacao} do usu�rio {IdUsuario} atualizada com sucesso - Status Code - {StatusCode}", idTransacao, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar transa��o {IdTransacao} para o usu�rio {IdUsuario}", idTransacao, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpDelete("usuario/{idUsuario}/transacao-contas/{idTransacao}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMovimentacaoFinanceira([Required][FromRoute] int idUsuario,
                                                                       [Required][FromRoute] int idTransacao)
        {
            try
            {
                var response = await _transacaoService.DeleteMovimentacaoFinanceira(idUsuario, idTransacao);
                if (_transacaoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao deletar transa��o {IdTransacao} do usu�rio {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idTransacao, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Transa��o {IdTransacao} do usu�rio {IdUsuario} deletada com sucesso - Status Code - {StatusCode}", idTransacao, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar transa��o {IdTransacao} do usu�rio {IdUsuario}", idTransacao, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}