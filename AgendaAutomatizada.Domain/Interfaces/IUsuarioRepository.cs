using AgendaAutomatizada.Domain.Entities;

namespace AgendaAutomatizada.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<UsuarioEntity> CriarUsuario(UsuarioEntity usuario);
    Task<UsuarioEntity?> ObterUsuarioPorEmail(string email);
    Task<UsuarioEntity?> ObterUsuarioPorCpf(string cpf);
}
