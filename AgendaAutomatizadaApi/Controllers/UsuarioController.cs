using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using FastEndpoints;

public class UsuarioController : Endpoint<UsuarioRequest, UsuarioResponse>
{
    public override void Configure()
    {
        Post("/api/usuario");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(UsuarioRequest requisicao, CancellationToken ct)
    {
        var resposta = new UsuarioResponse
        {
            IdUsuario = 1,
            Nome = requisicao.Nome,
            Email = requisicao.Email,
            Token = Guid.NewGuid().ToString()
        };

        await Send.OkAsync(resposta, cancellation: ct);
    }
    
    
}