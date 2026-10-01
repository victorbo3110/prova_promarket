namespace Promarket.Payments.Api.Core.Domain;

public class Pagamento
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int EventoId { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = "ok";
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

public class PagamentoRequest
{
    public int PedidoId { get; set; }
    public int EventoId { get; set; }
    public decimal Valor { get; set; }
}

public class AtualizarPagamentoRequest
{
    public int PedidoId { get; set; }
    public int EventoId { get; set; }
    public decimal Valor { get; set; }
}

public class PagamentoResponse
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int EventoId { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
