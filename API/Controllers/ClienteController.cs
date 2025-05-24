using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using API.ViewModel.Cliente;
using gerenciador.financas.Core.Services;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Extensions;

namespace gerenciador.financas.API.Controllers
{
    [ApiController]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [Route("dados/cliente/")]
        [HttpGet]
        public IActionResult ObterDadosCadastrais([Required] string cpf)
        {
            return Ok(new { Message = "Dados a b c d e" });
        }

        [Route("dados/cliente/")]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InserirDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel dadosPessoais)
        {
            var dadosPessoaisService = MapperProfile.ToServiceRequest(dadosPessoais);

            var response = _clienteService.InserirDadosCadastrais(dadosPessoaisService);

            return Ok(new { Message = "Entrada atualizada" });
        }

        [Route("dados/cliente/")]
        [HttpPut]
        public IActionResult AtualizarDadosCadastrais([Required][FromBody] DadosPessoaisRequestViewModel cadastrarEntradaViewModel)
        {
            return Ok(new { Message = "Entrada atualizada" });
        }

        [Route("login/cliente/")]
        [HttpPut]
        public IActionResult AtualizarLogin([Required][FromBody] AtualizarLoginRequestViewModel cadastrarEntradaViewModel)
        {
            return Ok(new { Message = "Entrada atualizada" });
        }

        [Route("dados/cliente/")]
        [HttpDelete]
        public IActionResult ExcluirDadosCadastrais([FromQuery][Required] string cpf)
        {
            return Ok(new { Message = "Entrada removida" });
        }
    }
}