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
    public class ReceitaController : Controller
    {
        private readonly IReceitaService _receitaService;
        private readonly NotificationPool _notificationPool;

        public ReceitaController(IReceitaService receitaService, 
                                 NotificationPool notificationPool)
        {
            _receitaService = receitaService;
            _notificationPool = notificationPool;
        }

        [HttpGet("receitas/cliente")]
        [ProducesResponseType(typeof(List<ReceitaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterReceitasUsuario([Required] int idUsuario, int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitas(idUsuario, periodo);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(r => r.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
        [HttpGet("receitas/categoria/cliente")]
        [ProducesResponseType(typeof(List<ReceitaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterReceitasUsuarioPorCategoria([Required] int idUsuario, int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitasPorCategoria(idUsuario, periodo);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(r => r.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpGet("receitas/conta/cliente")]
        [ProducesResponseType(typeof(List<ReceitaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterReceitasUsuarioPorConta([Required] int idUsuario, int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitasPorConta(idUsuario, periodo);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(r => r.ToViewModel())
                    .ToList();

                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
        [HttpGet("receitas/total/cliente")]
        [ProducesResponseType(typeof(List<ReceitaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTotalReceitasUsuario([Required] int idUsuario, int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitasTotalPorPeriodo(idUsuario, periodo);
                if (_receitaService.HasNotifications)
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

        [HttpPost("receita/cliente")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InserirReceitas([Required][FromBody] ReceitaRequestViewModel receitaRequest,
                                                         [Required]int idUsuario,
                                                         [Required]int idCategoria,
                                                         [Required] int idConta)

        {
            try
            {
                var response = await _receitaService.InsertReceita(receitaRequest, idUsuario , idCategoria, idConta); 
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();

                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);

                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                return Created(string.Empty, "receita inserida com sucesso");
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("receita/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarReceita([Required][FromBody] ReceitaRequestViewModel receitaRequest, 
                                                          [Required]int idUsuario,
                                                          [Required]int idReceita,
                                                          [Required]int idCategoria,
                                                          [Required]int idConta)
        {
            try
            {
                var response = await _receitaService.UpdateReceita(receitaRequest, idUsuario, idReceita, idCategoria, idConta);
                if (_receitaService.HasNotifications)
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

        [HttpDelete("receita/cliente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExcluirReceita([Required] int idUsuario, [Required] int idReceita)
        {
            try
            {
                var response = await _receitaService.DeleteReceita(idUsuario, idReceita);  
                if (_receitaService.HasNotifications)
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
