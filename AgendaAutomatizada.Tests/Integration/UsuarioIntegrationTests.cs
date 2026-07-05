using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Infrastructure.Data;

namespace AgendaAutomatizada.Tests.Integration;

public class UsuarioIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsuarioIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private static string GerarCpfAleatorio()
    {
        var random = new Random();
        return string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10).ToString()));
    }

    [Fact]
    public async Task CriarUsuario_ComDadosValidos_Retorna201()
    {
        var request = new UsuarioRequest
        {
            Nome = "João Silva",
            Email = $"joao{Guid.NewGuid()}@email.com",
            Senha = "123456",
            Cpf = GerarCpfAleatorio(),
            Telefone = "11999999999"
        };

        var response = await _client.PostAsJsonAsync("/api/usuarios", request);
        var usuario = await response.Content.ReadFromJsonAsync<UsuarioResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(usuario);
        Assert.Equal(request.Nome, usuario.Nome);
        Assert.Equal(request.Email, usuario.Email);
        Assert.True(usuario.IdUsuario > 0);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        var usuarioNoBanco = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

        Assert.NotNull(usuarioNoBanco);
        Assert.Equal(request.Nome, usuarioNoBanco!.Nome);
        Assert.Equal(request.Cpf, usuarioNoBanco.Cpf);
        Assert.NotEqual(request.Senha, usuarioNoBanco.SenhaHash);
    }

    [Fact]
    public async Task CriarUsuario_ComEmailDuplicado_Retorna409()
    {
        var email = $"duplicado{Guid.NewGuid()}@email.com";
        var cpf = GerarCpfAleatorio();

        var primeiroRequest = new UsuarioRequest
        {
            Nome = "João Silva",
            Email = email,
            Senha = "123456",
            Cpf = cpf,
            Telefone = "11999999999"
        };

        var primeiroResponse = await _client.PostAsJsonAsync("/api/usuarios", primeiroRequest);
        Assert.Equal(HttpStatusCode.Created, primeiroResponse.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
            Assert.True(await db.Usuarios.AnyAsync(u => u.Email == email));
        }

        var segundoRequest = new UsuarioRequest
        {
            Nome = "Maria Souza",
            Email = email,
            Senha = "123456",
            Cpf = GerarCpfAleatorio(),
            Telefone = "11988888888"
        };

        var response = await _client.PostAsJsonAsync("/api/usuarios", segundoRequest);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CriarUsuario_ComCpfDuplicado_Retorna409()
    {
        var email = $"cpfduplicado{Guid.NewGuid()}@email.com";
        var cpf = GerarCpfAleatorio();

        var primeiroRequest = new UsuarioRequest
        {
            Nome = "João Silva",
            Email = $"primeiro{Guid.NewGuid()}@email.com",
            Senha = "123456",
            Cpf = cpf,
            Telefone = "11999999999"
        };

        var primeiroResponse = await _client.PostAsJsonAsync("/api/usuarios", primeiroRequest);
        Assert.Equal(HttpStatusCode.Created, primeiroResponse.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
            Assert.True(await db.Usuarios.AnyAsync(u => u.Cpf == cpf));
        }

        var segundoRequest = new UsuarioRequest
        {
            Nome = "Maria Souza",
            Email = email,
            Senha = "123456",
            Cpf = cpf,
            Telefone = "11988888888"
        };

        var response = await _client.PostAsJsonAsync("/api/usuarios", segundoRequest);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CriarUsuario_ComDadosInvalidos_Retorna400()
    {
        var request = new UsuarioRequest
        {
            Nome = "",
            Email = "email-invalido",
            Senha = "12",
            Cpf = "123",
            Telefone = ""
        };

        var response = await _client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
        Assert.False(await db.Usuarios.AnyAsync());
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_Retorna200()
    {
        var email = $"login{Guid.NewGuid()}@email.com";
        var senha = "123456";

        var usuarioRequest = new UsuarioRequest
        {
            Nome = "João Silva",
            Email = email,
            Senha = senha,
            Cpf = GerarCpfAleatorio(),
            Telefone = "11999999999"
        };

        await _client.PostAsJsonAsync("/api/usuarios", usuarioRequest);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Senha = senha
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var usuario = await response.Content.ReadFromJsonAsync<UsuarioResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(usuario);
        Assert.Equal(email, usuario.Email);
    }

    [Fact]
    public async Task Login_ComEmailInvalido_Retorna401()
    {
        var loginRequest = new LoginRequest
        {
            Email = "inexistente@email.com",
            Senha = "123456"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ComSenhaInvalida_Retorna401()
    {
        var email = $"loginsenha{Guid.NewGuid()}@email.com";

        var usuarioRequest = new UsuarioRequest
        {
            Nome = "João Silva",
            Email = email,
            Senha = "123456",
            Cpf = GerarCpfAleatorio(),
            Telefone = "11999999999"
        };

        await _client.PostAsJsonAsync("/api/usuarios", usuarioRequest);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Senha = "senha_errada"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
