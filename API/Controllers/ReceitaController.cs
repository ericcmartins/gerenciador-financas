using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using gerenciador.financas.Infra.Vendors;
using Core.ViewModel.gerenciador.financas.API.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class ReceitaController : Controller
    {
        private readonly IReceitaService _receitaService;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<ReceitaController> _logger;

        public ReceitaController(IReceitaService receitaService,
                                   NotificationPool notificationPool,
                                   ILogger<ReceitaController> logger)
        {
            _receitaService = receitaService;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        [HttpGet("usuario/{idUsuario}/receitas")]
        [ProducesResponseType(typeof(List<ReceitaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReceitasPorUsuario([Required][FromRoute] int idUsuario,
                                                                                 int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitasPorUsuario(idUsuario, periodo);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar receitas do usuário {IdUsuario} para o período {Periodo}: Status Code - {StatusCode}, {Mensagem}", idUsuario, periodo, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(r => r.ToViewModel())
                    .ToList();

                _logger.LogInformation("Receitas do usuário {IdUsuario} para o período {Periodo} recuperadas com sucesso - Status Code - {StatusCode}", idUsuario, periodo, StatusCodes.Status200OK);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar receitas do usuário {IdUsuario} para o período {Periodo}", idUsuario, periodo);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpGet("usuario/{idUsuario}/receitas/categoria/total")]
        [ProducesResponseType(typeof(List<ReceitaPorCategoriaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReceitasTotaisUsuarioPorCategoria([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitasPorCategoria(idUsuario, periodo);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar totais de receitas por categoria do usuário {IdUsuario} para o período {Periodo}: Status Code - {StatusCode}, {Mensagem}", idUsuario, periodo, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(r => r.ToViewModel())
                    .ToList();

                _logger.LogInformation("Totais de receitas por categoria do usuário {IdUsuario} para o período {Periodo} recuperados com sucesso - Status Code - {StatusCode}", idUsuario, periodo, StatusCodes.Status200OK);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar totais de receitas por categoria do usuário {IdUsuario} para o período {Periodo}", idUsuario, periodo);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpGet("usuario/{idUsuario}/receitas/conta/total")]
        [ProducesResponseType(typeof(List<ReceitaPorContaResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalReceitasUsuarioPorConta([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitasPorConta(idUsuario, periodo);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar totais de receitas por conta do usuário {IdUsuario} para o período {Periodo}: Status Code - {StatusCode}, {Mensagem}", idUsuario, periodo, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(r => r.ToViewModel())
                    .ToList();

                _logger.LogInformation("Totais de receitas por conta do usuário {IdUsuario} para o período {Periodo} recuperados com sucesso - Status Code - {StatusCode}", idUsuario, periodo, StatusCodes.Status200OK);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar totais de receitas por conta do usuário {IdUsuario} para o período {Periodo}", idUsuario, periodo);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpGet("usuario/{idUsuario}/receitas/total")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalReceitasUsuario([Required][FromRoute] int idUsuario, int periodo)
        {
            try
            {
                var response = await _receitaService.GetReceitasTotalPorPeriodo(idUsuario, periodo);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar total de receitas do usuário {IdUsuario} para o período {Periodo}: Status Code - {StatusCode}, {Mensagem}", idUsuario, periodo, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Total de receitas do usuário {IdUsuario} para o período {Periodo} recuperado com sucesso - Status Code - {StatusCode}", idUsuario, periodo, StatusCodes.Status200OK);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar total de receitas do usuário {IdUsuario} para o período {Periodo}", idUsuario, periodo);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/{idUsuario}/receita")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertReceitas([Required][FromBody] CadastrarReceitaRequestViewModel cadastrarReceitaRequest,
                                                        [Required][FromRoute] int idUsuario,
                                                        [Required] int idCategoria,
                                                        [Required] int idConta)
        {
            try
            {
                var response = await _receitaService.InsertReceita(cadastrarReceitaRequest, idUsuario, idCategoria, idConta);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao inserir receita para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Receita para o usuário {IdUsuario} inserida com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status201Created);
                return Created(string.Empty, "Receita inserida com sucesso na base");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao inserir receita para o usuário {IdUsuario}", idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{idUsuario}/receita/{idReceita}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReceita([Required][FromBody] AtualizarReceitaRequestViewModel receitaRequest,
                                                        [Required][FromRoute] int idUsuario,
                                                        [Required][FromRoute] int idReceita,
                                                        [Required] int idCategoria,
                                                        [Required] int idConta)
        {
            try
            {
                var response = await _receitaService.UpdateReceita(receitaRequest, idUsuario, idReceita, idCategoria, idConta);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao atualizar receita {IdReceita} para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idReceita, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Receita {IdReceita} do usuário {IdUsuario} atualizada com sucesso - Status Code - {StatusCode}", idReceita, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar receita {IdReceita} para o usuário {IdUsuario}", idReceita, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpDelete("usuario/{idUsuario}/receita/{idReceita}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReceita([Required][FromRoute] int idUsuario,
                                                        [Required][FromRoute] int idReceita)
        {
            try
            {
                var response = await _receitaService.DeleteReceita(idUsuario, idReceita);
                if (_receitaService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao deletar receita {IdReceita} do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idReceita, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Receita {IdReceita} do usuário {IdUsuario} deletada com sucesso - Status Code - {StatusCode}", idReceita, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar receita {IdReceita} do usuário {IdUsuario}", idReceita, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}