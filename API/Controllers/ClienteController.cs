using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using API.ViewModel.Cliente;
using gerenciador.financas.Application.Services;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Extensions;
using Core.ViewModel;
using System.Net;
using gerenciador.financas.Domain.Entities.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.ViewModel.Cliente;
using gerenciador.financas.Domain.Utils;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("cliente/{cpf}/dados")]
        [ProducesResponseType(typeof(DadosPessoaisResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterDadosCadastrais([Required] string cpf)
        {
            if (!CpfValidator.ValidaCpf(cpf))
                return BadRequest("Cpf Inválido");

            try
            {
                var response = await _clienteService.GetDadosPessoais(cpf);

                if (response is null)
                {
                    return NotFound("Nenhuma informação corresponde ao cpf informado");
                }

                return Ok(response.ToViewModel());
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno");
            }
        }

        [HttpPost("cliente/dados")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InserirDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel dadosPessoais)
        {
            try
            {
                var dadosPessoaisService = MapperProfile.DadosPessoaisViewlModelToService(dadosPessoais);
                var response = await _clienteService.InserirDadosCadastrais(dadosPessoaisService);

                if (string.IsNullOrEmpty(response))
                    return BadRequest("Não foi possível inserir os dados.");

                return Created("cliente/dados", response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("cliente/{cpf}dados")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel dadosPessoais, [Required] string cpf)
        {
            try
            {
                var dadosPessoaisService = MapperProfile.DadosPessoaisViewlModelToService(dadosPessoais);
                var response = await _clienteService.UpdateDadosPessoais(dadosPessoaisService, cpf);

                if (string.IsNullOrEmpty(response))
                    return NotFound("Não foi possível atualizar os dados.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("cliente/{cpf}/senha")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarSenha([Required] string cpf, [Required][FromBody] string senha)
        {
            try
            {
                var response = await _clienteService.UpdateSenha(cpf, senha);

                if (string.IsNullOrEmpty(response))
                    return NotFound("Não foi possível atualizar a senha.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("cliente/{cpf}/email")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarEmail([Required] string cpf, [Required][FromBody] string email)
        {
            try
            {
                var response = await _clienteService.UpdateEmail(cpf, email);

                if (string.IsNullOrEmpty(response))
                    return NotFound("Não foi possível atualizar o email.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("cliente/{cpf}/telefone")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarTelefone([Required] string cpf, [Required][FromBody] string telefone)
        {
            try
            {
                var response = await _clienteService.UpdateTelefone(cpf, telefone);

                if (string.IsNullOrEmpty(response))
                    return NotFound("Não foi possível atualizar o telefone.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("cliente/{cpf}/dados")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExcluirDadosCadastrais([Required] string cpf)
        {
            try
            {
                var response = await _clienteService.DeleteConta(cpf);

                if (string.IsNullOrEmpty(response))
                    return NotFound("Não foi possível deletar os dados.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
