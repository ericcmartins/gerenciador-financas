using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using gerenciador.financas.Infra.Vendors;
using Core.ViewModel.gerenciador.financas.API.ViewModels;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly NotificationPool _notificationPool;
        private readonly IAuthService _authService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioService usuarioService,
                                   NotificationPool notificationPool,
                                   IAuthService authService,
                                   ILogger<UsuarioController> logger)
        {
            _usuarioService = usuarioService;
            _notificationPool = notificationPool;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("usuario/{id}")]
        [ProducesResponseType(typeof(DadosPessoaisResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDadosCadastrais([Required][FromRoute] int id)
        {
            try
            {
                var response = await _usuarioService.GetDadosPessoais(id);
                if (_usuarioService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao buscar dados cadastrais do usuário {Id}: Status Code - {StatusCode}, {Mensagem}", id, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Dados cadastrais do usuário {Id} recuperados com sucesso - Status Code - {StatusCode}", id, StatusCodes.Status200OK);
                return Ok(response.ToViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar dados cadastrais do usuário {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/cadastro")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertCadastroUsuario([Required][FromBody] CadastrarUsuarioRequestViewModel dadosCadastro)
        {
            try
            {
                var response = await _usuarioService.InsertCadastroUsuario(dadosCadastro);
                if (_usuarioService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao cadastrar usuário com email {Email}: Status Code - {StatusCode}, {Mensagem}", dadosCadastro.Email, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Usuário com email {Email} cadastrado com sucesso - Status Code - {StatusCode}", dadosCadastro.Email, StatusCodes.Status201Created);
                return Created(string.Empty, "Usuario inserido com sucesso na base");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao cadastrar usuário com email {Email}", dadosCadastro.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPut("usuario/{id}/cadastro")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDadosPessoais([Required][FromRoute] int id,
                                                             [Required][FromBody] AtualizarDadosCadastraisRequestViewModel atualizarCadastro)
        {
            try
            {
                var response = await _usuarioService.UpdateDadosPessoais(atualizarCadastro, id);
                if (_usuarioService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao atualizar dados pessoais do usuário {Id}: Status Code - {StatusCode}, {Mensagem}", id, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Dados pessoais do usuário {Id} atualizados com sucesso - Status Code - {StatusCode}", id, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar dados pessoais do usuário {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpDelete("usuario/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDadosCadastrais([Required][FromRoute] int id)
        {
            try
            {
                var response = await _usuarioService.DeleteConta(id);
                if (_usuarioService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao deletar usuário {Id}: Status Code - {StatusCode}, {Mensagem}", id, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Usuário {Id} deletado com sucesso - Status Code - {StatusCode}", id, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao deletar usuário {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost("usuario/login")]
        [ProducesResponseType(typeof(LoginResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([Required][FromBody] LoginRequestViewModel loginRequest)
        {
            try
            {
                var response = await _authService.RealizarLogin(loginRequest.Email, loginRequest.Senha);

                if (_authService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao realizar login para o email {Email}: Status Code - {StatusCode}, {Mensagem}", loginRequest.Email, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Login para o email {Email} realizado com sucesso - Status Code - {StatusCode}", loginRequest.Email, StatusCodes.Status200OK);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao realizar login para o email {Email}", loginRequest.Email);
                return StatusCode(500, new ErrorViewModel(500, $"Erro interno: {ex.Message}"));
            }
        }

        [HttpPut("usuario/alterarSenha")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AlterarSenha([Required][FromBody] AtualizarSenhaRequestViewModel alterarSenhaRequest)
        {
            try
            {
                var response = await _usuarioService.AlterarSenha(alterarSenhaRequest.Email, alterarSenhaRequest.NovaSenha, alterarSenhaRequest.Telefone);
                if (_usuarioService.HasNotifications)
                {
                    var notificacao = _notificationPool.Notifications.First();
                    _logger.LogWarning("Falha ao alterar senha para o email {Email}: Status Code - {StatusCode}, {Mensagem}", alterarSenhaRequest.Email, notificacao.StatusCode, notificacao.Mensagem);
                    var errorViewModel = new ErrorViewModel(notificacao.StatusCode, notificacao.Mensagem);
                    return StatusCode(errorViewModel.StatusCode, errorViewModel);
                }

                _logger.LogInformation("Senha para o email {Email} alterada com sucesso - Status Code - {StatusCode}", alterarSenhaRequest.Email, StatusCodes.Status204NoContent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao alterar senha para o email {Email}", alterarSenhaRequest.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro interno: {ex.Message}");
            }
        }
    }
}