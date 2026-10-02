using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Promarket.Payments.Api.Tests;

public class PagamentosApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PagamentosApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostPagamentos_ShouldCreatePayment()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/pagamentos", new
        {
            id = 101,
            pedidoId = 10,
            eventoId = 20,
            valor = 99.90m
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(payload);
        Assert.True(payload!.ContainsKey("idPagamento"));
        Assert.True(payload.ContainsKey("status"));
    }

    [Fact]
    public async Task GetPagamentos_ShouldReturnOrderedList()
    {
        var client = _factory.CreateClient();

        await client.PostAsJsonAsync("/pagamentos", new { id = 201, pedidoId = 1, eventoId = 10, valor = 10.00m });
        await client.PostAsJsonAsync("/pagamentos", new { id = 202, pedidoId = 2, eventoId = 20, valor = 20.00m });

        var response = await client.GetAsync("/pagamentos?ordem=asc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagamentos = await response.Content.ReadFromJsonAsync<List<PagamentoResponse>>();
        Assert.NotNull(pagamentos);
        Assert.True(pagamentos!.Count >= 2);
    }

    [Fact]
    public async Task PostPagamentos_WithSameRequestIdWithinShortInterval_ShouldFailWithoutDuplication()
    {
        var client = _factory.CreateClient();
        var requestId = 777;

        var first = await client.PostAsJsonAsync("/pagamentos", new
        {
            id = requestId,
            pedidoId = 30,
            eventoId = 99,
            valor = 80.50m
        });

        var second = await client.PostAsJsonAsync("/pagamentos", new
        {
            id = requestId,
            pedidoId = 31,
            eventoId = 99,
            valor = 80.50m
        });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        var listResponse = await client.GetAsync("/pagamentos?ordem=desc");
        var pagamentos = await listResponse.Content.ReadFromJsonAsync<List<PagamentoResponse>>();

        Assert.NotNull(pagamentos);
        Assert.Equal(1, pagamentos!.Count(p => p.EventoId == 99));
    }

    private sealed class PagamentoResponse
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int EventoId { get; set; }
        public decimal Valor { get; set; }
    }
}