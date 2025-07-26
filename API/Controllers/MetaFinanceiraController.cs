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

        [HttpGet("usuario/{idUsuario}/metas")]
        [ProducesResponseType(typeof(List<MetaFinanceiraResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMetasFinanceiras([Required][FromQuery] int idUsuario)
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

        [HttpPost("usuario/{idUsuario}/meta")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertMetaFinanceira([Required][FromBody] CadastrarMetaFinanceiraRequestViewModel metaFinanceiraRequest, 
                                                              [Required][FromRoute] int idUsuario)
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

                return Created(string.Empty, "meta financeira inserida com sucesso");
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{idUsuario}/meta/{idMetaFinanceira}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMetaFinanceira([Required][FromBody] AtualizarMetaFinanceiraRequestViewModel metaFinanceiraRequest,
                                                              [Required][FromRoute] int idUsuario, 
                                                              [Required][FromRoute] int idMetaFinanceira)
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

        [HttpDelete("usuario/{idUsuario}/meta/{idMetaFinanceira}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteMetaFinanceira([Required][FromRoute] int idMetaFinanceira, 
                                                              [Required][FromRoute] int idUsuario)
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
