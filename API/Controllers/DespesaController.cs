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

        [HttpGet("despesas/cliente")]
        [ProducesResponseType(typeof(List<DespesaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterDespesasUsuario([Required] int idUsuario, int periodo)
        {
            try
            {
                var response = await _despesaService.GetDespesas(idUsuario, periodo);
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

        [HttpPost("despesa/cliente")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InserirDespesa([Required][FromBody] DespesaRequestViewModel despesaRequest,
                                                         [Required]int idUsuario,                                                       
                                                         [Required]int idCategoria,
                                                         [Required]int idConta,
                                                         [Required]int idMetodoPagamento)

        {
            try
            {
                var response = await _despesaService.InsertDespesa(despesaRequest, idUsuario, idCategoria, idConta, idMetodoPagamento);
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

        [HttpPut("despesa/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarDespesa([Required][FromBody] DespesaRequestViewModel despesaRequest, 
                                                          [Required]int idUsuario,
                                                          [Required]int idDespesa,
                                                          [Required]int idCategoria,
                                                          [Required]int idConta,
                                                          [Required]int idMetodoPagamento)
        {
            try
            {
                var response = await _despesaService.UpdateDespesa(despesaRequest, idUsuario, idDespesa, idCategoria, idConta, idMetodoPagamento);
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

        [HttpDelete("despesa/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ExcluirDespesa([Required] int idUsuario, [Required] int idDespesa)
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
