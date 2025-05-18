using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public class EntradasController : ControllerBase
{
    [Route("entradas/cliente/{cpf}")]
    [HttpGet]
    public IActionResult ObterEntradas([Required] string cpf,
                                      [FromQuery][Required] string intervaloDias,
                                      [FromQuery] string categoria,
                                      [FromQuery] string metodoPagamento)
    {
        return Ok(new { Message = "Entrada do mês é de uma milha" });
    }

    [Route("entradas/cliente/{cpf}")]
    [HttpPost]
    public IActionResult InserirEntrada([Required] string cpf,
                                      [FromBody] CadastrarEntradaViewModel cadastrarEntradaViewModel)
    {
        return Ok(new { Message = "Entrada do mês é de uma milha" });
    }

    [Route("entradas/cliente/{cpf}")]
    [HttpPut]
    public IActionResult AtualizarEntradas([Required] string cpf, 
                                          [FromQuery] string id,
                                          [FromBody] CadastrarEntradaViewModel cadastrarEntradaViewModel)
    {
        return Ok(new { Message = "Entrada atualizada" });
    }

    [Route("entradas/cliente/{cpf}")]
    [HttpDelete]
    public IActionResult ExcluirEntradas([Required] string cpf, 
                                        [FromQuery] string id)
    {
        return Ok(new { Message = "Entrada removida" });
    }
}