using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Service.Services;
using AgendaAutomatizadaApi.Mappers;
using FastEndpoints;

public class CriarUsuarioEndpoint : Endpoint<UsuarioRequest, UsuarioResponse>
{
    private readonly UsuarioService _usuarioService;
    private readonly ITokenService _tokenService;

    public CriarUsuarioEndpoint(UsuarioService usuarioService, ITokenService tokenService)
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
    }

    public override void Configure()
    {
        Post("/api/usuario");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UsuarioRequest usuarioRequisicao, CancellationToken ct)
    {
        try
        {
            var usuarioEntity = usuarioRequisicao.ToEntity();

            var usuario = await _usuarioService.CriarUsuario(usuarioEntity, usuarioRequisicao.Senha);

            var token = _tokenService.GerarToken(usuario);
            var resposta = usuario.ToResponse(token);

            await Send.ResponseAsync(resposta, statusCode: 201, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}

