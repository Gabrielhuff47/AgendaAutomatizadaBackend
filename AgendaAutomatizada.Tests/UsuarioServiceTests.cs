using Moq;
using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Service.Services;

namespace AgendaAutomatizada.Tests;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly UsuarioService _usuarioService;

    public UsuarioServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task CriarUsuario_DadosValidos_RetornaUsuarioCriado()
    {
        // Arrange
        var usuario = new UsuarioEntity
        {
            Nome = "João",
            Email = "joao@email.com",
            Cpf = "12345678901",
            Telefone = "11999999999"
        };

        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorEmail(usuario.Email))
            .ReturnsAsync((UsuarioEntity?)null);

        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorCpf(usuario.Cpf))
            .ReturnsAsync((UsuarioEntity?)null);

        _passwordHasherMock.Setup(ph => ph.Hash(It.IsAny<string>()))
            .Returns("senha_hash_criptografada");

        _usuarioRepositoryMock.Setup(repo => repo.CriarUsuario(It.IsAny<UsuarioEntity>()))
            .ReturnsAsync((UsuarioEntity u) => u);

        // Act
        var resultado = await _usuarioService.CriarUsuario(usuario, "senha123");

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(usuario.Nome, resultado.Dados!.Nome);
        Assert.Equal(usuario.Email, resultado.Dados!.Email);
        Assert.Equal("senha_hash_criptografada", resultado.Dados!.SenhaHash);
    }

    [Fact]
    public async Task CriarUsuario_EmailJaExiste_RetornaFalha()
    {
        // Arrange
        var usuarioExistente = new UsuarioEntity { Email = "joao@email.com" };
        var novoUsuario = new UsuarioEntity { Email = "joao@email.com", Cpf = "12345678901" };

        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorEmail("joao@email.com"))
            .ReturnsAsync(usuarioExistente);

        // Act
        var resultado = await _usuarioService.CriarUsuario(novoUsuario, "senha123");

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("E-mail já cadastrado.", resultado.Mensagem);
    }

    [Fact]
    public async Task CriarUsuario_CpfJaExiste_RetornaFalha()
    {
        // Arrange
        var usuarioExistente = new UsuarioEntity { Cpf = "12345678901" };
        var novoUsuario = new UsuarioEntity { Email = "joao@email.com", Cpf = "12345678901" };

        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorEmail(novoUsuario.Email))
            .ReturnsAsync((UsuarioEntity?)null);

        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorCpf("12345678901"))
            .ReturnsAsync(usuarioExistente);

        // Act
        var resultado = await _usuarioService.CriarUsuario(novoUsuario, "senha123");

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("CPF já cadastrado.", resultado.Mensagem);
    }

    [Fact]
    public async Task AutenticarUsuario_EmailESenhaValidos_RetornaUsuario()
    {
        // Arrange
        var usuario = new UsuarioEntity
        {
            IdUsuario = 1,
            Nome = "João",
            Email = "joao@email.com",
            SenhaHash = "hash_criptografado"
        };

        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorEmail("joao@email.com"))
            .ReturnsAsync(usuario);

        _passwordHasherMock.Setup(ph => ph.Verify("senha123", "hash_criptografado"))
            .Returns(true);

        // Act
        var resultado = await _usuarioService.AutenticarUsuario("joao@email.com", "senha123");

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(usuario.Email, resultado.Dados!.Email);
        Assert.Equal(usuario.Nome, resultado.Dados!.Nome);
    }

    [Fact]
    public async Task AutenticarUsuario_EmailInvalido_RetornaFalha()
    {
        // Arrange
        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorEmail("invalido@email.com"))
            .ReturnsAsync((UsuarioEntity?)null);

        // Act
        var resultado = await _usuarioService.AutenticarUsuario("invalido@email.com", "senha123");

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("Entre com um usuário e senha corretos", resultado.Mensagem);
    }

    [Fact]
    public async Task AutenticarUsuario_SenhaInvalida_RetornaFalha()
    {
        // Arrange
        var usuario = new UsuarioEntity
        {
            Email = "joao@email.com",
            SenhaHash = "hash_correto"
        };

        _usuarioRepositoryMock.Setup(repo => repo.ObterUsuarioPorEmail("joao@email.com"))
            .ReturnsAsync(usuario);

        _passwordHasherMock.Setup(ph => ph.Verify("senha_errada", "hash_correto"))
            .Returns(false);

        // Act
        var resultado = await _usuarioService.AutenticarUsuario("joao@email.com", "senha_errada");

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("Entre com um usuário e senha corretos", resultado.Mensagem);
    }
}
