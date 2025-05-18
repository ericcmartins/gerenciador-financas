using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using API.ViewModel.Cliente;

namespace API.Controllers;
[ApiController]
public class ClienteController : Controller
{
    [Route("dados/cliente/")]
    [HttpGet]
    public IActionResult ObterDadosCadastrais([Required] string cpf)
    {
        return Ok(new { Message = "Dados a b c d e" });
    }
    
    [Route("dados/cliente/")]
    [HttpPost]
    public IActionResult InserirDadosCadastrais([Required][FromBody] DadosCadastraisRequestViewModel cadastrarEntradaViewModel)
    {
        return Ok(new { Message = "Entrada do mês é de uma milha" });
    }

    [Route("dados/cliente/")]
    [HttpPut]
    public IActionResult AtualizarDadosCadastrais([Required][FromBody] DadosCadastraisRequestViewModel cadastrarEntradaViewModel)
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