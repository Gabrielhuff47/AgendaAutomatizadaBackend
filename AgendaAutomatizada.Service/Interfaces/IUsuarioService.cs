using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Service.Shared;

namespace AgendaAutomatizada.Service.Interfaces;

public interface IUsuarioService
{
    Task<Result<UsuarioEntity>> CriarUsuario(UsuarioEntity usuario, string senha);
    Task<Result<UsuarioEntity>> AutenticarUsuario(string email, string senha);
}
