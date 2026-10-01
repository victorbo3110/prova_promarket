using Microsoft.AspNetCore.Mvc;
using Promarket.Payments.Api.Core.Application;
using Promarket.Payments.Api.Core.Domain;

namespace Promarket.Payments.Api.Controllers;

[ApiController]
[Route("pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly PagamentoService _service;

    public PagamentosController(PagamentoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] PagamentoRequest request)
    {
        try
        {
            var pagamento = await _service.RegistrarAsync(request);
            return Ok(new { status = "ok", idPagamento = pagamento.Id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Status = "Inválido", Mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Status = "Inválido", Mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<PagamentoResponse>>> ObterTodos([FromQuery] string ordem = "desc")
    {
        var pagamentos = await _service.ObterTodosAsync(ordem);
        return Ok(pagamentos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PagamentoResponse>> ObterPorId(int id)
    {
        var pagamento = await _service.ObterPorIdAsync(id);
        if (pagamento is null)
            return NotFound();

        return Ok(pagamento);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Atualizar(int id, [FromBody] AtualizarPagamentoRequest request)
    {
        try
        {
            var pagamento = await _service.AtualizarAsync(id, request);
            return Ok(new { status = "ok", idPagamento = pagamento.Id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Status = "Inválido", Mensagem = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Status = "Inválido", Mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Status = "Inválido", Mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Excluir(int id)
    {
        var removido = await _service.ExcluirAsync(id);
        if (!removido)
            return NotFound(new { Status = "Inválido", Mensagem = "Pagamento não encontrado." });

        return NoContent();
    }
}
