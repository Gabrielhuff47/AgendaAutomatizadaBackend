using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Infrastructure.Data;
using AgendaAutomatizadaApi;

namespace AgendaAutomatizada.Tests.Integration;

public class TarefaIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public TarefaIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CriarTarefa_ComDadosValidos_Retorna201()
    {
        var request = new TarefaRequest
        {
            Titulo = "Reuniao de planejamento",
            Descricao = "Discutir metas do proximo trimestre",
            Data = DateTime.UtcNow.AddDays(1)
        };

        var response = await _client.PostAsJsonAsync("/api/tarefas", request);
        var tarefa = await response.Content.ReadFromJsonAsync<TarefaResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(tarefa);
        Assert.Equal(request.Titulo, tarefa.Titulo);
        Assert.Equal(request.Descricao, tarefa.Descricao);
        Assert.True(tarefa.IdTarefa > 0);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        var tarefaNoBanco = await dbContext.Tarefas.FirstOrDefaultAsync(t => t.IdTarefa == tarefa.IdTarefa);

        Assert.NotNull(tarefaNoBanco);
        Assert.Equal(request.Titulo, tarefaNoBanco!.Titulo);
        Assert.Equal(request.Descricao, tarefaNoBanco.Descricao);
    }

    [Fact]
    public async Task CriarTarefa_ComDadosInvalidos_Retorna400()
    {
        var request = new TarefaRequest
        {
            Titulo = "",
            Descricao = "",
            Data = DateTime.MinValue
        };

        var response = await _client.PostAsJsonAsync("/api/tarefas", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        Assert.False(await db.Tarefas.AnyAsync());
    }

    [Fact]
    public async Task ListarTarefas_QuandoVazia_Retorna200ComListaVazia()
    {
        var response = await _client.GetAsync("/api/tarefas");
        var tarefas = await response.Content.ReadFromJsonAsync<List<TarefaResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tarefas);
        Assert.Empty(tarefas);
    }

    [Fact]
    public async Task ListarTarefas_ComTarefasCriadas_RetornaListaComTarefas()
    {
        var request1 = new TarefaRequest
        {
            Titulo = "Primeira tarefa",
            Descricao = "Descricao da primeira",
            Data = DateTime.UtcNow.AddDays(1)
        };
        var request2 = new TarefaRequest
        {
            Titulo = "Segunda tarefa",
            Descricao = "Descricao da segunda",
            Data = DateTime.UtcNow.AddDays(2)
        };

        await _client.PostAsJsonAsync("/api/tarefas", request1);
        await _client.PostAsJsonAsync("/api/tarefas", request2);

        var response = await _client.GetAsync("/api/tarefas");
        var tarefas = await response.Content.ReadFromJsonAsync<List<TarefaResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tarefas);
        Assert.Equal(2, tarefas.Count);
        Assert.Contains(tarefas, t => t.Titulo == request1.Titulo);
        Assert.Contains(tarefas, t => t.Titulo == request2.Titulo);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        Assert.Equal(2, await db.Tarefas.CountAsync());
    }

    [Fact]
    public async Task ObterTarefaPorId_Existente_Retorna200()
    {
        var criarRequest = new TarefaRequest
        {
            Titulo = "Tarefa especifica",
            Descricao = "Descricao da tarefa",
            Data = DateTime.UtcNow.AddDays(1)
        };

        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas", criarRequest);
        var tarefaCriada = await criarResponse.Content.ReadFromJsonAsync<TarefaResponse>();

        var response = await _client.GetAsync($"/api/tarefas/{tarefaCriada!.IdTarefa}");
        var tarefa = await response.Content.ReadFromJsonAsync<TarefaResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tarefa);
        Assert.Equal(tarefaCriada.IdTarefa, tarefa.IdTarefa);
        Assert.Equal(criarRequest.Titulo, tarefa.Titulo);
    }

    [Fact]
    public async Task ObterTarefaPorId_Inexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/tarefas/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AtualizarTarefa_Existente_Retorna200()
    {
        var criarRequest = new TarefaRequest
        {
            Titulo = "Tarefa original",
            Descricao = "Descricao original",
            Data = DateTime.UtcNow.AddDays(1)
        };

        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas", criarRequest);
        var tarefaCriada = await criarResponse.Content.ReadFromJsonAsync<TarefaResponse>();

        var atualizarRequest = new TarefaRequest
        {
            Titulo = "Tarefa atualizada",
            Descricao = "Descricao atualizada",
            Data = DateTime.UtcNow.AddDays(2)
        };

        var response = await _client.PutAsJsonAsync($"/api/tarefas/{tarefaCriada!.IdTarefa}", atualizarRequest);
        var tarefa = await response.Content.ReadFromJsonAsync<TarefaResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tarefa);
        Assert.Equal(tarefaCriada.IdTarefa, tarefa.IdTarefa);
        Assert.Equal(atualizarRequest.Titulo, tarefa.Titulo);
        Assert.Equal(atualizarRequest.Descricao, tarefa.Descricao);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        var tarefaNoBanco = await db.Tarefas.FirstOrDefaultAsync(t => t.IdTarefa == tarefaCriada.IdTarefa);

        Assert.NotNull(tarefaNoBanco);
        Assert.Equal(atualizarRequest.Titulo, tarefaNoBanco!.Titulo);
        Assert.Equal(atualizarRequest.Descricao, tarefaNoBanco.Descricao);
    }

    [Fact]
    public async Task AtualizarTarefa_Inexistente_Retorna404()
    {
        var atualizarRequest = new TarefaRequest
        {
            Titulo = "Tarefa inexistente",
            Descricao = "Nao deve atualizar",
            Data = DateTime.UtcNow.AddDays(1)
        };

        var response = await _client.PutAsJsonAsync("/api/tarefas/99999", atualizarRequest);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        Assert.False(await db.Tarefas.AnyAsync());
    }

    [Fact]
    public async Task DeletarTarefa_Existente_Retorna200()
    {
        var criarRequest = new TarefaRequest
        {
            Titulo = "Tarefa para deletar",
            Descricao = "Sera removida",
            Data = DateTime.UtcNow.AddDays(1)
        };

        var criarResponse = await _client.PostAsJsonAsync("/api/tarefas", criarRequest);
        var tarefaCriada = await criarResponse.Content.ReadFromJsonAsync<TarefaResponse>();
        var deleteRequest = new DeletarTarefaRequest { idTarefa = tarefaCriada!.IdTarefa };
        var response = await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"/api/tarefas/{tarefaCriada.IdTarefa}", UriKind.Relative),
            Content = JsonContent.Create(deleteRequest)
        });
        var result = await response.Content.ReadFromJsonAsync<DeletarTarefaResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.Sucesso);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        Assert.False(await db.Tarefas.AnyAsync());
    }

    [Fact]
    public async Task DeletarTarefa_Inexistente_Retorna404()
    {
        var deleteRequest = new DeletarTarefaRequest { idTarefa = 99999 };
        var response = await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/api/tarefas/99999", UriKind.Relative),
            Content = JsonContent.Create(deleteRequest)
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        Assert.False(await db.Tarefas.AnyAsync());
    }
}
