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
    [Route("[controller]")]
    public class DespesaController : ControllerBase
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
        [ProducesResponseType(typeof(DadosPessoaisResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterDadosCadastrais([Required] int idUsuario)
        {
            try
            {
                var response = await _usuarioService.GetDadosPessoais(idUsuario);
                if (_notificationPool.HasNotifications())
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Ok(response.ToViewModel());
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
        public async Task<IActionResult> InserirDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel dadosPessoais)
        {
            try
            {
                 var response = await _usuarioService.InsertDadosPessoais(dadosPessoais);
                if (_notificationPool.HasNotifications())
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

        [HttpPut("despesa/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel dadosPessoais, [Required]int idUsuario)
        {
            try
            {
                 var response = await _usuarioService.UpdateDadosPessoais(dadosPessoais, idUsuario);
                if (_notificationPool.HasNotifications())
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

        public async Task<IActionResult> ExcluirDadosCadastrais([Required][FromQuery]int idUsuario)
        {
            try
            {
                 var response = await _usuarioService.DeleteConta(idUsuario);
                if (_notificationPool.HasNotifications())
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
