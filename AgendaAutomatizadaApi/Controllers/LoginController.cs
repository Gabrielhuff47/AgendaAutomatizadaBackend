using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Service.Services;
using AgendaAutomatizadaApi.Mappers;
using FastEndpoints;

public class LoginEndpoint : Endpoint<LoginRequest, UsuarioResponse>
{
    private readonly UsuarioService _usuarioService;

    public LoginEndpoint(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public override void Configure()
    {
        Post("/api/usuario/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest requisicao, CancellationToken ct)
    {
        try
        {
            var usuario = await _usuarioService.AutenticarUsuario(requisicao.Email, requisicao.Senha);

            var resposta = usuario.ToResponse(Guid.NewGuid().ToString());

            await Send.OkAsync(resposta, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
