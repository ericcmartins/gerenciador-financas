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

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class PagamentoController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<PagamentoController> _logger;

        public PagamentoController(IPagamentoService metodoPagamentoService,
                                   NotificationPool notificationPool,
                                   ILogger<PagamentoController> logger)
        {
            _pagamentoService = metodoPagamentoService;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        [HttpGet("usuario/{idUsuario}/metodosPagamento")]
        [ProducesResponseType(typeof(List<MetodoPagamentoResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMetodosPagamentoUsuario([Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _pagamentoService.GetMetodosPagamentoUsuario(idUsuario);
                if (_pagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar métodos de pagamento do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(m => m.ToViewModel())
                    .ToList();

                _logger.LogInformation("Métodos de pagamento do usuário {IdUsuario} recuperados com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status200OK);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar métodos de pagamento do usuário {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/{idUsuario}/metodoPagamento")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertMetodoPagamento([Required][FromBody] CadastrarMetodoPagamentoRequestViewModel inserirMetodoPagamentoRequest,
                                                               [Required][FromRoute] int idUsuario,
                                                               [Required] int idConta)
        {
            try
            {
                var response = await _pagamentoService.InsertMetodoPagamento(inserirMetodoPagamentoRequest, idUsuario, idConta);
                if (_pagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao inserir método de pagamento para o usuário {IdUsuario} na conta {IdConta}: Status Code - {StatusCode}, {Mensagem}", idUsuario, idConta, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Método de pagamento para o usuário {IdUsuario} inserido com sucesso na conta {IdConta} - Status Code - {StatusCode}", idUsuario, idConta, StatusCodes.Status201Created);
                return Created(string.Empty, "Método de pagamento inserido com sucesso na base");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao inserir método de pagamento para o usuário {IdUsuario} na conta {IdConta}", idUsuario, idConta);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{idUsuario}/metodoPagamento/{idMetodoPagamento}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMetodoPagamento([Required][FromBody] AtualizarMetodoPagamentoRequestViewModel atualizarMetodoPagamentoRequest,
                                                               [Required][FromRoute] int idUsuario,
                                                               [Required][FromRoute] int idMetodoPagamento,
                                                               [Required] int idConta)
        {
            try
            {
                var response = await _pagamentoService.UpdateMetodoPagamento(atualizarMetodoPagamentoRequest, idUsuario, idConta, idMetodoPagamento);
                if (_pagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao atualizar método de pagamento {IdMetodoPagamento} para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idMetodoPagamento, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Método de pagamento {IdMetodoPagamento} do usuário {IdUsuario} atualizado com sucesso - Status Code - {StatusCode}", idMetodoPagamento, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar método de pagamento {IdMetodoPagamento} para o usuário {IdUsuario}", idMetodoPagamento, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpDelete("usuario/{idUsuario}/metodoPagamento/{idMetodoPagamento}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMetodoPagamento([Required][FromRoute] int idUsuario,
                                                               [Required][FromRoute] int idMetodoPagamento)
        {
            try
            {
                var response = await _pagamentoService.DeleteMetodoPagamento(idUsuario, idMetodoPagamento);
                if (_pagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao deletar método de pagamento {IdMetodoPagamento} do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idMetodoPagamento, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Método de pagamento {IdMetodoPagamento} do usuário {IdUsuario} deletado com sucesso - Status Code - {StatusCode}", idMetodoPagamento, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar método de pagamento {IdMetodoPagamento} do usuário {IdUsuario}", idMetodoPagamento, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}