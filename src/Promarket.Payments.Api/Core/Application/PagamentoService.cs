using Microsoft.EntityFrameworkCore;
using Promarket.Payments.Api.Core.Domain;
using Promarket.Payments.Api.Data;

namespace Promarket.Payments.Api.Core.Application;

public class PagamentoService
{
    private readonly PagamentoDbContext _context;

    public PagamentoService(PagamentoDbContext context)
    {
        _context = context;
    }

    public async Task<Pagamento> RegistrarAsync(PagamentoRequest request)
    {
        if (request is null)
            throw new ArgumentException("Requisição inválida.");

        ValidarRequest(request);

        var existente = await _context.Pagamentos
            .FirstOrDefaultAsync(p => p.PedidoId == request.PedidoId && p.EventoId == request.EventoId);

        if (existente is not null)
            return existente;

        var pagamento = new Pagamento
        {
            PedidoId = request.PedidoId,
            EventoId = request.EventoId,
            Valor = request.Valor,
            Status = "ok",
            CriadoEm = DateTime.UtcNow
        };

        _context.Pagamentos.Add(pagamento);
        await _context.SaveChangesAsync();
        return pagamento;
    }

    public async Task<List<PagamentoResponse>> ObterTodosAsync(string ordem = "desc")
    {
        var query = _context.Pagamentos.AsQueryable();

        query = string.Equals(ordem, "asc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(p => p.Id)
            : query.OrderByDescending(p => p.Id);

        return await query
            .Select(p => new PagamentoResponse
            {
                Id = p.Id,
                PedidoId = p.PedidoId,
                EventoId = p.EventoId,
                Valor = p.Valor,
                Status = p.Status,
                CriadoEm = p.CriadoEm
            })
            .ToListAsync();
    }

    public async Task<PagamentoResponse?> ObterPorIdAsync(int id)
    {
        return await _context.Pagamentos
            .Where(p => p.Id == id)
            .Select(p => new PagamentoResponse
            {
                Id = p.Id,
                PedidoId = p.PedidoId,
                EventoId = p.EventoId,
                Valor = p.Valor,
                Status = p.Status,
                CriadoEm = p.CriadoEm
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Pagamento> AtualizarAsync(int id, AtualizarPagamentoRequest request)
    {
        if (request is null)
            throw new ArgumentException("Requisição inválida.");

        ValidarRequest(request);

        var pagamento = await _context.Pagamentos.FirstOrDefaultAsync(p => p.Id == id);
        if (pagamento is null)
            throw new KeyNotFoundException("Pagamento não encontrado.");

        var jaExiste = await _context.Pagamentos
            .AnyAsync(p => p.Id != id && p.PedidoId == request.PedidoId && p.EventoId == request.EventoId);

        if (jaExiste)
            throw new InvalidOperationException("Já existe um pagamento para este pedido e evento.");

        pagamento.PedidoId = request.PedidoId;
        pagamento.EventoId = request.EventoId;
        pagamento.Valor = request.Valor;
        pagamento.Status = "ok";

        await _context.SaveChangesAsync();
        return pagamento;
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        var pagamento = await _context.Pagamentos.FirstOrDefaultAsync(p => p.Id == id);
        if (pagamento is null)
            return false;

        _context.Pagamentos.Remove(pagamento);
        await _context.SaveChangesAsync();
        return true;
    }

    private static void ValidarRequest(PagamentoRequest request)
    {
        if (request.PedidoId <= 0)
            throw new ArgumentException("pedidoId deve ser maior que zero.");

        if (request.EventoId <= 0)
            throw new ArgumentException("eventoId deve ser maior que zero.");

        if (request.Valor <= 0)
            throw new ArgumentException("valor deve ser maior que zero.");
    }

    private static void ValidarRequest(AtualizarPagamentoRequest request)
    {
        if (request.PedidoId <= 0)
            throw new ArgumentException("pedidoId deve ser maior que zero.");

        if (request.EventoId <= 0)
            throw new ArgumentException("eventoId deve ser maior que zero.");

        if (request.Valor <= 0)
            throw new ArgumentException("valor deve ser maior que zero.");
    }
}
