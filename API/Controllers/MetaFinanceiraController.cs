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
    public class MetaFinanceiraController : Controller
    {
        private readonly IMetaFinanceiraService _metaFinanceiraService;
        private readonly NotificationPool _notificationPool;

        public MetaFinanceiraController(IMetaFinanceiraService metaFinanceiraService, 
                                 NotificationPool notificationPool)
        {
            _metaFinanceiraService = metaFinanceiraService;
            _notificationPool = notificationPool;
        }

        [HttpGet("metas/cliente")]
        [ProducesResponseType(typeof(List<MetaFinanceiraResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMetasFinanceiras([Required] int idUsuario)
        {
            try
            {
                var response = await _metaFinanceiraService.GetMetasFinanceiras(idUsuario);
                if (_metaFinanceiraService.HasNotifications)
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

        [HttpPost("meta/cliente")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertMetaFinanceira([Required][FromBody] MetaFinanceiraRequestViewModel metaFinanceiraRequest, [Required]int idUsuario)
        {
            try
            {
                 var response = await _metaFinanceiraService.InsertMetaFinanceira(metaFinanceiraRequest, idUsuario);
                if (_metaFinanceiraService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Created();
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("meta/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMetaFinanceira([Required][FromBody] MetaFinanceiraRequestViewModel metaFinanceiraRequest,
                                                                  [Required] int idUsuario, 
                                                                  [Required]int idMetaFinanceira)
        {
            try
            {
                 var response = await _metaFinanceiraService.UpdateMetaFinanceira(metaFinanceiraRequest, idUsuario, idMetaFinanceira);
                if (_metaFinanceiraService.HasNotifications)
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

        [HttpDelete("meta/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteMetaFinanceira([Required]int idMetaFinanceira, [Required]int idUsuario)
        {
            try
            {
                 var response = await _metaFinanceiraService.DeleteMetaFinanceira(idMetaFinanceira, idUsuario);
                if (_metaFinanceiraService.HasNotifications)
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
