using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Domain.Interfaces;

namespace AgendaAutomatizada.Service.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(IUsuarioRepository repository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioEntity> CriarUsuario(UsuarioEntity usuario, string senha)
    {
        var emailExists = await _usuarioRepository.ObterUsuarioPorEmail(usuario.Email);
        if (emailExists is not null)
            throw new InvalidOperationException("E-mail já cadastrado.");

        var cpfExists = await _usuarioRepository.ObterUsuarioPorCpf(usuario.Cpf);
        if (cpfExists is not null)
            throw new InvalidOperationException("CPF já cadastrado.");

        usuario.SenhaHash = _passwordHasher.Hash(senha);
        usuario.DataCriacao = DateTime.UtcNow;
        usuario.UsuarioAtualizacao = usuario.Nome;
        usuario.DataAtualizacao = DateTime.UtcNow;

        return await _usuarioRepository.CriarUsuario(usuario);
    }

    public async Task<UsuarioEntity> AutenticarUsuario(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmail(email);
        if (usuario is null)
            throw new InvalidOperationException("entre com um usuário e senha corretos");

        var senhaValida = _passwordHasher.Verify(senha, usuario.SenhaHash);
        if (!senhaValida)
            throw new InvalidOperationException("entre com um usuário e senha corretos");

        return usuario;
    }
}
