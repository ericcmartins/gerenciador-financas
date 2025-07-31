using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using gerenciador.financas.Infra.Vendors;
using Core.ViewModel.gerenciador.financas.API.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class ContaController : Controller
    {
        private readonly IContaService _contaService;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<ContaController> _logger;

        public ContaController(IContaService contaService,
                                   NotificationPool notificationPool,
                                   ILogger<ContaController> logger)
        {
            _contaService = contaService;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        [HttpGet("usuario/{idUsuario}/contas")]
        [ProducesResponseType(typeof(List<ContaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContasUsuario([Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _contaService.GetContasPorUsuario(idUsuario);
                if (_contaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    _logger.LogWarning("Falha ao buscar contas do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(c => c.ToViewModel())
                    .ToList();

                _logger.LogInformation("Contas do usuário {IdUsuario} recuperadas com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status200OK);
                return Ok(viewModel);
            }

            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar contas do usuário {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/{idUsuario}/conta")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertConta([Required][FromBody] CadastrarContaRequestViewModel contaRequest,
                                                     [Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _contaService.InsertConta(contaRequest, idUsuario);
                if (_contaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    _logger.LogWarning("Falha ao inserir conta para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Conta para o usuário {IdUsuario} inserida com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status201Created);
                return Created(string.Empty, "Conta inserida com sucesso na base");
            }

            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao inserir conta para o usuário {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{idUsuario}/conta/{idConta}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateConta([Required][FromBody] AtualizarContaRequestViewModel atualizarContaRequest,
                                                     [Required][FromRoute] int idConta,
                                                     [Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _contaService.UpdateConta(atualizarContaRequest, idUsuario, idConta);
                if (_contaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    _logger.LogWarning("Falha ao atualizar conta {IdConta} para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idConta, idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Conta {IdConta} do usuário {IdUsuario} atualizada com sucesso - Status Code - {StatusCode}", idConta, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }

            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar conta {IdConta} para o usuário {IdUsuario}", idConta, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpDelete("usuario/{idUsuario}/conta/{idConta}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteConta([Required][FromRoute] int idConta,
                                                     [Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _contaService.DeleteConta(idConta, idUsuario);
                if (_contaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    _logger.LogWarning("Falha ao deletar conta {IdConta} do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idConta, idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Conta {IdConta} do usuário {IdUsuario} deletada com sucesso - Status Code - {StatusCode}", idConta, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }

            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar conta {IdConta} do usuário {IdUsuario}", idConta, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}