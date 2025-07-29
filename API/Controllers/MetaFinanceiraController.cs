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
    public class MetaFinanceiraController : Controller
    {
        private readonly IMetaFinanceiraService _metaFinanceiraService;
        private readonly NotificationPool _notificationPool;
        private readonly ILogger<MetaFinanceiraController> _logger;

        public MetaFinanceiraController(IMetaFinanceiraService metaFinanceiraService,
                                   NotificationPool notificationPool,
                                   ILogger<MetaFinanceiraController> logger)
        {
            _metaFinanceiraService = metaFinanceiraService;
            _notificationPool = notificationPool;
            _logger = logger;
        }

        [HttpGet("usuario/{idUsuario}/metas")]
        [ProducesResponseType(typeof(List<MetaFinanceiraResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMetasFinanceiras([Required][FromRoute] int idUsuario)
        {
            try
            {
                var response = await _metaFinanceiraService.GetMetasFinanceiras(idUsuario);
                if (_metaFinanceiraService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar metas financeiras do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                var viewModel = response
                    .Select(mf => mf.ToViewModel())
                    .ToList();

                _logger.LogInformation("Metas financeiras do usuário {IdUsuario} recuperadas com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status200OK);
                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar metas financeiras do usuário {IdUsuario}", idUsuario);
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
                    _logger.LogWarning("Falha ao inserir meta financeira para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Meta financeira para o usuário {IdUsuario} inserida com sucesso - Status Code - {StatusCode}", idUsuario, StatusCodes.Status201Created);
                return Created(string.Empty, "meta financeira inserida com sucesso");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao inserir meta financeira para o usuário {IdUsuario}", idUsuario);
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
                    _logger.LogWarning("Falha ao atualizar meta financeira {IdMetaFinanceira} para o usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idMetaFinanceira, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Meta financeira {IdMetaFinanceira} do usuário {IdUsuario} atualizada com sucesso - Status Code - {StatusCode}", idMetaFinanceira, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar meta financeira {IdMetaFinanceira} para o usuário {IdUsuario}", idMetaFinanceira, idUsuario);
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
                    _logger.LogWarning("Falha ao deletar meta financeira {IdMetaFinanceira} do usuário {IdUsuario}: Status Code - {StatusCode}, {Mensagem}", idMetaFinanceira, idUsuario, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Meta financeira {IdMetaFinanceira} do usuário {IdUsuario} deletada com sucesso - Status Code - {StatusCode}", idMetaFinanceira, idUsuario, StatusCodes.Status204NoContent);
                return NoContent();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar meta financeira {IdMetaFinanceira} do usuário {IdUsuario}", idMetaFinanceira, idUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}