using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Service.Shared;

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

    public async Task<Result<UsuarioEntity>> CriarUsuario(UsuarioEntity usuario, string senha)
    {
        var emailExists = await _usuarioRepository.ObterUsuarioPorEmail(usuario.Email);
        if (emailExists is not null)
            return Result<UsuarioEntity>.Falha("E-mail já cadastrado.", TipoErro.Conflito);

        var cpfExists = await _usuarioRepository.ObterUsuarioPorCpf(usuario.Cpf);
        if (cpfExists is not null)
            return Result<UsuarioEntity>.Falha("CPF já cadastrado.", TipoErro.Conflito);

        usuario.SenhaHash = _passwordHasher.Hash(senha);
        usuario.DataCriacao = DateTime.UtcNow;
        usuario.UsuarioAtualizacao = usuario.Nome;
        usuario.DataAtualizacao = DateTime.UtcNow;

        var usuarioCriado = await _usuarioRepository.CriarUsuario(usuario);
        return Result<UsuarioEntity>.Ok(usuarioCriado);
    }

    public async Task<Result<UsuarioEntity>> AutenticarUsuario(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmail(email);
        if (usuario is null)
            return Result<UsuarioEntity>.Falha("Entre com um usuário e senha corretos", TipoErro.NaoAutorizado);

        var senhaValida = _passwordHasher.Verify(senha, usuario.SenhaHash);
        if (!senhaValida)
            return Result<UsuarioEntity>.Falha("Entre com um usuário e senha corretos", TipoErro.NaoAutorizado);

        return Result<UsuarioEntity>.Ok(usuario);
    }
}
