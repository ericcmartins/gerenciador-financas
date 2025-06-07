using Core.ViewModel.gerenciador.financas.API.ViewModels;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Infra.Vendors;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly NotificationPool _notificationPool;

        public CategoriaController(ICategoriaService categoriaService,
                                   NotificationPool notificationPool)
        {
            _categoriaService = categoriaService;
            _notificationPool = notificationPool;
        }

        [HttpGet("categorias/cliente")]
        [ProducesResponseType(typeof(CategoriaResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterCategoriasUsuario([Required] int idUsuario)
        {
            try
            {
                var response = await _categoriaService.GetCategorias(idUsuario);
                if (_categoriaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(c => c.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("categoria/cliente")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InserirCategoria([Required][FromBody] CategoriaRequestViewModel categoriaRequest, int idUsuario)
        {
            try
            {
                var response = await _categoriaService.InsertCategoria(categoriaRequest, idUsuario);
                if (_categoriaService.HasNotifications)
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

        [HttpPut("categoria/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarCategoria([Required][FromBody] CategoriaRequestViewModel categoriaRequest, int idCategoria, int idUsuario)
        {
            try
            {
                var response = await _categoriaService.UpdateCategoria(categoriaRequest, idCategoria, idUsuario);
                if (_categoriaService.HasNotifications)
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

        [HttpDelete("categoria/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ExcluirDadosCadastrais([Required]int idCategoria, [Required] int idUsuario)
        {
            try
            {
                var response = await _categoriaService.DeleteCategoria(idCategoria, idUsuario);
                if (_categoriaService.HasNotifications)
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
