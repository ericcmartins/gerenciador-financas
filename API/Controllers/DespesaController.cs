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
    public class DespesaController : Controller
    {
        private readonly IDespesaService _despesaService;
        private readonly NotificationPool _notificationPool;

        public DespesaController(IDespesaService despesaService,
                                 NotificationPool notificationPool)
        {
            _despesaService = despesaService;
            _notificationPool = notificationPool;
        }

        [HttpGet("usuario/{idUsuario}/despesas")]
        [ProducesResponseType(typeof(List<DespesaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDespesasPorUsuario([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _despesaService.GetDespesasPorUsuario(idUsuario, periodo);
                if (_despesaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(d => d.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpGet("usuario/{idUsuario}/despesas/categoria")]
        [ProducesResponseType(typeof(List<DespesaPorCategoriaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDespesasUsuarioPorCategoria([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _despesaService.GetDespesasPorCategoria(idUsuario, periodo);
                if (_despesaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(d => d.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpGet("usuario/{idUsuario}/despesas/conta")]
        [ProducesResponseType(typeof(List<DespesaPorContaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDespesasUsuarioPorConta([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _despesaService.GetDespesasPorConta(idUsuario, periodo);
                if (_despesaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(d => d.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
        [HttpGet("usuario/{idUsuario}/despesas/metodoPagamento")]
        [ProducesResponseType(typeof(List<DespesaPorMetodoPagamentoResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDespesasUsuarioPorMetodoPagamento([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _despesaService.GetDespesasPorMetodoPagamento(idUsuario, periodo);
                if (_despesaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(d => d.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
        [HttpGet("usuario/{idUsuario}/despesas/total")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalDespesasUsuarioPorPeriodo([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _despesaService.GetTotalDespesasPeriodo(idUsuario, periodo);
                if (_despesaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Ok(response);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/{idUsuario}/despesa")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertDespesa([Required][FromBody] CadastrarDespesaRequestViewModel despesaRequest,
                                                         [Required][FromRoute] int idUsuario,
                                                         [Required] int? idCategoria,
                                                         [Required] int idMetodoPagamento)

        {
            try
            {
                var response = await _despesaService.InsertDespesa(despesaRequest, idUsuario, idCategoria, idMetodoPagamento);
                if (_despesaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Created(string.Empty, "despesa inserida com sucesso");
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{idUsuario}/despesa/{idDespesa}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDespesa([Required][FromBody] AtualizarDespesaRequestViewModel despesaRequest,
                                                          [Required][FromRoute] int idUsuario,
                                                          [Required][FromRoute] int idDespesa,
                                                          [Required] int? idCategoria,
                                                          [Required] int idMetodoPagamento)
        {
            try
            {
                var response = await _despesaService.UpdateDespesa(despesaRequest, idUsuario, idDespesa, idCategoria, idMetodoPagamento);
                if (_despesaService.HasNotifications)
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

        [HttpDelete("usuario/{idUsuario}/despesa/{idDespesa}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteDespesa([Required][FromRoute] int idUsuario, 
                                                       [Required][FromRoute] int idDespesa)
        {
            try
            {
                var response = await _despesaService.DeleteDespesa(idUsuario, idDespesa);
                if (_despesaService.HasNotifications)
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
