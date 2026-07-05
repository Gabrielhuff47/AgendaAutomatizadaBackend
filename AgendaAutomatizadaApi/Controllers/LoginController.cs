using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Service.Interfaces;
using AgendaAutomatizadaApi.Mappers;
using FastEndpoints;

public class LoginEndpoint : Endpoint<LoginRequest, UsuarioResponse>
{
    private readonly IUsuarioService _usuarioService;

    public LoginEndpoint(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(LoginRequest requisicao, CancellationToken ct)
    {
        var resultado = await _usuarioService.AutenticarUsuario(requisicao.Email, requisicao.Senha);

        if (await resultado.SendErrorIfFailedAsync(HttpContext, ct))
            return;

        var resposta = resultado.Dados!.ToResponse();
        await Send.OkAsync(resposta, cancellation: ct);
    }
}
