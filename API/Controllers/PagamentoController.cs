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
    public class PagamentoController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly NotificationPool _notificationPool;

        public PagamentoController(IPagamentoService metodoPagamentoService, 
                                 NotificationPool notificationPool)
        {
            _pagamentoService = metodoPagamentoService;
            _notificationPool = notificationPool;
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

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Created(string.Empty, "Método de pagamento inserido com sucesso na base");
            }

            catch (Exception ex)
            {
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
