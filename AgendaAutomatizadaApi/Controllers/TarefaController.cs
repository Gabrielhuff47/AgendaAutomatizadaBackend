using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Service.Services;
using AgendaAutomatizada.Service.Shared;
using AgendaAutomatizadaApi.Mappers;
using FastEndpoints;

public class CriarTarefaEndpoint : Endpoint<TarefaRequest, TarefaResponse>
{
    private readonly TarefaService _tarefaService;

    public CriarTarefaEndpoint(TarefaService tarefaService)
    {
        _tarefaService = tarefaService;
    }

    public override void Configure()
    {
        Post("/api/tarefas");
        AllowAnonymous();
        Tags("Tarefas");
    }

    public override async Task HandleAsync(TarefaRequest tarefaRequisicao, CancellationToken ct)
    {
        var tarefaEntity = tarefaRequisicao.ToEntity();

        var resultado = await _tarefaService.CriarTarefa(tarefaEntity);

        if (await resultado.SendErrorIfFailedAsync(HttpContext, ct))
            return;

        var resposta = resultado.Dados!.ToResponse();
        await Send.ResponseAsync(resposta, statusCode: 201, cancellation: ct);
    }
}

public class ListarTarefasEndpoint : EndpointWithoutRequest<List<TarefaResponse>>
{
    private readonly TarefaService _tarefaService;

    public ListarTarefasEndpoint(TarefaService tarefaService)
    {
        _tarefaService = tarefaService;
    }

    public override void Configure()
    {
        Get("/api/tarefas");
        AllowAnonymous();
        Tags("Tarefas");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var resultado = await _tarefaService.ListarTarefas();

        if (await resultado.SendErrorIfFailedAsync(HttpContext, ct))
            return;

        var resposta = resultado.Dados!.Select(t => t.ToResponse()).ToList();
        await Send.ResponseAsync(resposta, cancellation: ct);
    }
}

public class ObterTarefaEndpoint : EndpointWithoutRequest<TarefaResponse>
{
    private readonly TarefaService _tarefaService;

    public ObterTarefaEndpoint(TarefaService tarefaService)
    {
        _tarefaService = tarefaService;
    }

    public override void Configure()
    {
        Get("/api/tarefas/{id}");
        AllowAnonymous();
        Tags("Tarefas");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");

        var resultado = await _tarefaService.ObterTarefaPorId(id);

        if (await resultado.SendErrorIfFailedAsync(HttpContext, ct))
            return;

        var resposta = resultado.Dados!.ToResponse();
        await Send.ResponseAsync(resposta, cancellation: ct);
    }
}

public class AtualizarTarefaEndpoint : Endpoint<TarefaRequest, TarefaResponse>
{
    private readonly TarefaService _tarefaService;

    public AtualizarTarefaEndpoint(TarefaService tarefaService)
    {
        _tarefaService = tarefaService;
    }

    public override void Configure()
    {
        Put("/api/tarefas/{id}");
        AllowAnonymous();
        Tags("Tarefas");
    }

    public override async Task HandleAsync(TarefaRequest tarefaRequisicao, CancellationToken ct)
    {
        var id = Route<int>("id");
        var tarefaEntity = tarefaRequisicao.ToEntity();

        var resultado = await _tarefaService.AtualizarTarefaPorId(id, tarefaEntity);

        if (await resultado.SendErrorIfFailedAsync(HttpContext, ct))
            return;

        var resposta = resultado.Dados!.ToResponse();
        await Send.ResponseAsync(resposta, cancellation: ct);
    }
}

public class DeletarTarefaEndpoint : EndpointWithoutRequest
{
    private readonly TarefaService _tarefaService;

    public DeletarTarefaEndpoint(TarefaService tarefaService)
    {
        _tarefaService = tarefaService;
    }

    public override void Configure()
    {
        Delete("/api/tarefas/{id}");
        AllowAnonymous();
        Tags("Tarefas");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");

        var resultado = await _tarefaService.DeletarTarefaPorId(id);

        if (await resultado.SendErrorIfFailedAsync(HttpContext, ct))
            return;

        await Send.ResponseAsync(null, statusCode: 204, cancellation: ct);
    }
}
