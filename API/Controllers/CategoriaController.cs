using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Extensions;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Application.ViewModel.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet("cliente/{cpf}/dados")]
        [ProducesResponseType(typeof(DadosPessoaisResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterDadosCadastrais([Required] int idUsuario)
        {
            try
            {
                var response = await _usuarioService.GetDadosPessoais(idUsuario);

                if (response is null)
                {
                    return NotFound("Nenhuma informa��o corresponde ao cpf informado");
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

        [HttpPut("cliente/{cpf}dados")]
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

        [HttpDelete("cliente/{cpf}/dados")]
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
