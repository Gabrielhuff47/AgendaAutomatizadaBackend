using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Service.Interfaces;
using AgendaAutomatizada.Service.Shared;
using AgendaAutomatizadaApi.Mappers;
using FastEndpoints;

public class CriarUsuarioEndpoint : Endpoint<UsuarioRequest, UsuarioResponse>
{
    private readonly IUsuarioService _usuarioService;

    public CriarUsuarioEndpoint(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public override void Configure()
    {
        Post("/api/usuarios");
        AllowAnonymous();
        Tags("Usuarios");
    }

    public override async Task HandleAsync(UsuarioRequest usuarioRequisicao, CancellationToken ct)
    {
        var usuarioEntity = usuarioRequisicao.ToEntity();

        var resultado = await _usuarioService.CriarUsuario(usuarioEntity, usuarioRequisicao.Senha);

        if (await resultado.SendErrorIfFailedAsync(HttpContext, ct))
            return;

        var resposta = resultado.Dados!.ToResponse();
        await Send.ResponseAsync(resposta, statusCode: 201, cancellation: ct);
    }
}

