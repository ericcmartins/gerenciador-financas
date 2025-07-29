using Core.ViewModel.gerenciador.financas.API.ViewModels;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Infra.Vendors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(ICategoriaService categoriaService,
                                     NotificationPool notificationPool,
                                     ILogger<CategoriaController> logger)
        {
            _categoriaService = categoriaService;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        [HttpGet("usuario/{idUsuario}/categorias")]
        [ProducesResponseType(typeof(List<CategoriaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoriasUsuario([Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _categoriaService.GetCategoriasPorUsuario(idUsuario);
                if (_categoriaService.HasNotifications)
                {

                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar categorias do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}: ", idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(c => c.ToViewModel())
                    .ToList();

                _logger.LogInformation("Categorias do usuário {IdUsuario} recuperadas com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status200OK);
                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar categorias do usuário {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/{idUsuario}/categoria")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertCategoria([Required][FromBody] CadastrarCategoriaRequestViewModel categoriaRequest, int idUsuario)
        {
            try
            {
                var response = await _categoriaService.InsertCategoria(categoriaRequest, idUsuario);
                if (_categoriaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    _logger.LogWarning("Falha ao inserir categoria para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Categoria para o usuário {IdUsuario} inserida com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status201Created);
                return Created(string.Empty, "categoria inserida com sucesso");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao inserir categoria para o usuário {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{idUsuario}/categoria/{idCategoria}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoria([Required][FromBody] AtualizarCategoriaRequestViewModel categoriaRequest,
                                                         [Required][FromRoute] int idCategoria,
                                                         [Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _categoriaService.UpdateCategoria(categoriaRequest, idCategoria, idUsuario);
                if (_categoriaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    _logger.LogWarning("Falha ao atualizar categoria {IdCategoria} para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idCategoria, idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Categoria {IdCategoria} do usuário {IdUsuario} atualizada com sucesso - Status Code - {StatusCode}", idCategoria, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar categoria {IdCategoria} para o usuário {IdUsuario}", idCategoria, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpDelete("usuario/{idUsuario}/categoria/{idCategoria}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDadosCadastrais([Required][FromRoute] int idCategoria,
                                                               [Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _categoriaService.DeleteCategoria(idCategoria, idUsuario);
                if (_categoriaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    _logger.LogWarning("Falha ao deletar categoria {IdCategoria} do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idCategoria, idUsuario, notificacao.StatusCode, notificacao.Mensagem);

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Categoria {IdCategoria} do usuário {IdUsuario} deletada com sucesso - Status Code - {StatusCode}", idCategoria, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar categoria {IdCategoria} do usuário {IdUsuario}", idCategoria, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}