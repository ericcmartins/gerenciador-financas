using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public class SaidasController : ControllerBase
{
    [Route("saidas/cliente")]
    [HttpGet]
    public IActionResult ObterSaidas()
    {
        return Ok(new { Message = "Saidinha" });
    }

    [Route("saidas/cliente")]
    [HttpPost]
    public IActionResult InserirSaida()
    {
        return Ok(new { Message = "Saida do mês saida" });
    }

    [Route("saidas/cliente/{id}")]
    [HttpPut]
    public IActionResult AtualizarSaida(string id)
    {
        return Ok(new { Message = "Saida atualizada" });
    }

    [Route("saidas/cliente/{id}")]
    [HttpDelete]
    public IActionResult ExcluirSaida(string id)
    {
        return Ok(new { Message = "Saida removida" });
    }
}