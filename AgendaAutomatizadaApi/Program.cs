using System.Text.Json;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Infrastructure.Data;
using AgendaAutomatizada.Infrastructure.Repositories;
using AgendaAutomatizada.Service.Interfaces;
using AgendaAutomatizada.Service.Services;
using AgendaAutomatizada.Service.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITarefaService, TarefaService>();
builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AgendaDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();
app.UseFastEndpoints(c =>
{
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new
        {
            statusCode,
            mensagem = "Não foi possível processar a requisição. Corrija os erros abaixo:",
            erros = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => JsonNamingPolicy.CamelCase.ConvertName(g.Key),
                    g => g.Select(f => f.ErrorMessage).ToArray()
                )
        };
    };
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
