using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using gerenciador.financas.Infra.Vendors.Notification;
using Core.ViewModel.gerenciador.financas.API.ViewModels;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly NotificationPool _notificationPool;

        public UsuarioController(IUsuarioService usuarioService, 
                                 NotificationPool notificationPool)
        {
            _usuarioService = usuarioService;
            _notificationPool = notificationPool;
        }

        [HttpGet("usuario/{id}/dados")]
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

        [HttpPost("usuario/dados")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InserirDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel dadosPessoais)
        {
            try
            {
                var response = await _usuarioService.InsertDadosPessoais(dadosPessoais);

                if (!response)
                    return BadRequest("Ocorreu um erro interno");

                return Created("cliente/dados", response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("usuario/{id}/dados")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel dadosPessoais, [Required] int idUsuario)
        {
            try
            {
                var response = await _usuarioService.UpdateDadosPessoais(dadosPessoais, idUsuario);

                if (!response)
                    return BadRequest("Ocorreu um erro interno");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("usuario/{id}/dados")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExcluirDadosCadastrais([Required] int idUsuario)
        {
            try
            {
                var response = await _usuarioService.DeleteConta(idUsuario);

                if (!response)
                    return BadRequest("Ocorreu um erro interno");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
