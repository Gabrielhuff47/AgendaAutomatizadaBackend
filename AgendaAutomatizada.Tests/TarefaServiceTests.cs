using Moq;
using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Service.Services;

namespace AgendaAutomatizada.Tests;

public class TarefaServiceTests
{
    private readonly Mock<ITarefaRepository> _tarefaRepositoryMock;
    private readonly TarefaService _tarefaService;

    public TarefaServiceTests()
    {
        _tarefaRepositoryMock = new Mock<ITarefaRepository>();
        _tarefaService = new TarefaService(_tarefaRepositoryMock.Object);
    }

    [Fact]
    public async Task CriarTarefa_RetornaTarefaCriada()
    {
        // Arrange
        var tarefa = new TarefaEntity
        {
            Titulo = "Minha tarefa",
            Descricao = "Descrição da tarefa",
            Data = new DateTime(2026, 07, 15)
        };

        _tarefaRepositoryMock.Setup(repo => repo.CriarTarefa(It.IsAny<TarefaEntity>()))
            .ReturnsAsync((TarefaEntity t) => t);

        // Act
        var resultado = await _tarefaService.CriarTarefa(tarefa);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(tarefa.Titulo, resultado.Dados!.Titulo);
        Assert.Equal(tarefa.Descricao, resultado.Dados!.Descricao);
    }

    [Fact]
    public async Task ListarTarefas_RetornaListaDeTarefas()
    {
        // Arrange
        var tarefas = new List<TarefaEntity>
        {
            new() { IdTarefa = 1, Titulo = "Tarefa 1", Descricao = "Desc 1" },
            new() { IdTarefa = 2, Titulo = "Tarefa 2", Descricao = "Desc 2" }
        };

        _tarefaRepositoryMock.Setup(repo => repo.ListarTarefas())
            .ReturnsAsync(tarefas);

        // Act
        var resultado = await _tarefaService.ListarTarefas();

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(2, resultado.Dados!.Count);
    }

    [Fact]
    public async Task ObterTarefaPorId_TarefaExiste_RetornaTarefa()
    {
        // Arrange
        var tarefa = new TarefaEntity { IdTarefa = 1, Titulo = "Tarefa", Descricao = "Descrição" };

        _tarefaRepositoryMock.Setup(repo => repo.ObterTarefaPorId(1))
            .ReturnsAsync(tarefa);

        // Act
        var resultado = await _tarefaService.ObterTarefaPorId(1);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal(tarefa.IdTarefa, resultado.Dados!.IdTarefa);
    }

    [Fact]
    public async Task ObterTarefaPorId_TarefaNaoExiste_RetornaFalha()
    {
        // Arrange
        _tarefaRepositoryMock.Setup(repo => repo.ObterTarefaPorId(It.IsAny<int>()))
            .ReturnsAsync((TarefaEntity?)null);

        // Act
        var resultado = await _tarefaService.ObterTarefaPorId(999);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("Nenhuma tarefa encontrada", resultado.Mensagem);
    }

    [Fact]
    public async Task AtualizarTarefaPorId_TarefaExiste_RetornaTarefaAtualizada()
    {
        // Arrange
        var tarefaExistente = new TarefaEntity
        {
            IdTarefa = 1,
            Titulo = "Titulo antigo",
            Descricao = "Desc antiga",
            Data = new DateTime(2026, 07, 10)
        };

        var tarefaAtualizada = new TarefaEntity
        {
            Titulo = "Titulo novo",
            Descricao = "Desc nova",
            Data = new DateTime(2026, 07, 20)
        };

        _tarefaRepositoryMock.Setup(repo => repo.ObterTarefaPorId(1))
            .ReturnsAsync(tarefaExistente);

        _tarefaRepositoryMock.Setup(repo => repo.AtualizarTarefa(It.IsAny<TarefaEntity>()))
            .ReturnsAsync((TarefaEntity t) => t);

        // Act
        var resultado = await _tarefaService.AtualizarTarefaPorId(1, tarefaAtualizada);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.Equal("Titulo novo", resultado.Dados!.Titulo);
        Assert.Equal("Desc nova", resultado.Dados!.Descricao);
    }

    [Fact]
    public async Task AtualizarTarefaPorId_TarefaNaoExiste_RetornaFalha()
    {
        // Arrange
        _tarefaRepositoryMock.Setup(repo => repo.ObterTarefaPorId(It.IsAny<int>()))
            .ReturnsAsync((TarefaEntity?)null);

        // Act
        var resultado = await _tarefaService.AtualizarTarefaPorId(999, new TarefaEntity());

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("Nenhuma tarefa encontrada", resultado.Mensagem);
    }

    [Fact]
    public async Task DeletarTarefaPorId_TarefaExiste_RetornaSucesso()
    {
        // Arrange
        _tarefaRepositoryMock.Setup(repo => repo.DeletarTarefaPorId(1))
            .ReturnsAsync(true);

        // Act
        var resultado = await _tarefaService.DeletarTarefaPorId(1);

        // Assert
        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task DeletarTarefaPorId_TarefaNaoExiste_RetornaFalha()
    {
        // Arrange
        _tarefaRepositoryMock.Setup(repo => repo.DeletarTarefaPorId(It.IsAny<int>()))
            .ReturnsAsync(false);

        // Act
        var resultado = await _tarefaService.DeletarTarefaPorId(999);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("Nenhuma tarefa encontrada", resultado.Mensagem);
    }
}
