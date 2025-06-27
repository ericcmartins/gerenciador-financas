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
    public class MetodoPagamentoController : Controller
    {
        private readonly IMetodoPagamentoService _metodoPagamentoService;
        private readonly NotificationPool _notificationPool;

        public MetodoPagamentoController(IMetodoPagamentoService metodoPagamentoService, 
                                 NotificationPool notificationPool)
        {
            _metodoPagamentoService = metodoPagamentoService;
            _notificationPool = notificationPool;
        }

        [HttpGet("metodospagamento")]
        [ProducesResponseType(typeof(List<MetodoPagamentoResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMetodosPagamentoUsuario([Required] int idUsuario)
        {
            try
            {
                var response = await _metodoPagamentoService.GetMetodosPagamentoUsuario(idUsuario);
                if (_metodoPagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(m => m.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("metodopagamento")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertMetodoPagamento([Required][FromBody] MetodoPagamentoRequestViewModel metodoPagamentoRequest, [Required]int idUsuario, [Required] int idConta)
        {
            try
            {
                 var response = await _metodoPagamentoService.InsertMetodoPagamento(metodoPagamentoRequest, idUsuario, idConta);
                if (_metodoPagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Created(string.Empty, "método de pagamento inserido com sucesso");
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("metodopagamento")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMetodoPagamento([Required][FromBody] MetodoPagamentoRequestViewModel metodoPagamentoRequest, 
                                                                  [Required] int idUsuario, 
                                                                  [Required] int idConta,
                                                                  [Required] int idMetodoPagamento)
        {
            try
            {
                 var response = await _metodoPagamentoService.UpdateMetodoPagamento(metodoPagamentoRequest, idUsuario, idConta, idMetodoPagamento);
                if (_metodoPagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return NoContent();
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpDelete("metodopagamento")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteMetodoPagamento([Required][FromQuery]int idUsuario, [Required]int idMetodoPagamento)
        {
            try
            {
                 var response = await _metodoPagamentoService.DeleteMetodoPagamento(idUsuario, idMetodoPagamento);
                if (_metodoPagamentoService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return NoContent();
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}
