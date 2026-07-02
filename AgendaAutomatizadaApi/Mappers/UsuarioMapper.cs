using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Domain.Entities;

namespace AgendaAutomatizadaApi.Mappers;

public static class UsuarioMapper
{
    public static UsuarioEntity ToEntity(this UsuarioRequest request)
    {
        return new UsuarioEntity
        {
            Nome = request.Nome,
            Email = request.Email,
            Cpf = request.Cpf,
            Telefone = request.Telefone
        };
    }

    public static UsuarioResponse ToResponse(this UsuarioEntity usuario, string token)
    {
        return new UsuarioResponse
        {
            IdUsuario = usuario.IdUsuario,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Token = token
        };
    }
}
