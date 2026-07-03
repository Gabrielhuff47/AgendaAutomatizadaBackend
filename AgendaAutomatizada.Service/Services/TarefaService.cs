using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Service.Interfaces;
using AgendaAutomatizada.Service.Shared;

namespace AgendaAutomatizada.Service.Services;

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _tarefaRepository;

    public TarefaService(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<Result<TarefaEntity>> CriarTarefa(TarefaEntity tarefa)
    {
        tarefa.DataCriacao = DateTime.UtcNow;
        tarefa.DataAtualizacao = DateTime.UtcNow;
        tarefa.UsuarioAtualizacao = "Sistema";

        var tarefaCriada = await _tarefaRepository.CriarTarefa(tarefa);
        return Result<TarefaEntity>.Ok(tarefaCriada);
    }

    public async Task<Result<List<TarefaEntity>>> ListarTarefas()
    {
        var tarefas = await _tarefaRepository.ListarTarefas();
        return Result<List<TarefaEntity>>.Ok(tarefas);
    }

    public async Task<Result<TarefaEntity>> ObterTarefaPorId(int id)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorId(id);
        if (tarefa is null)
            return Result<TarefaEntity>.Falha("Nenhuma tarefa encontrada", TipoErro.NaoEncontrado);

        return Result<TarefaEntity>.Ok(tarefa);
    }

    public async Task<Result<TarefaEntity>> AtualizarTarefaPorId(int id, TarefaEntity tarefaAtualizada)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorId(id);
        if (tarefa is null)
            return Result<TarefaEntity>.Falha("Nenhuma tarefa encontrada", TipoErro.NaoEncontrado);

        tarefa.Titulo = tarefaAtualizada.Titulo;
        tarefa.Descricao = tarefaAtualizada.Descricao;
        tarefa.Data = tarefaAtualizada.Data;
        tarefa.DataAtualizacao = DateTime.UtcNow;
        tarefa.UsuarioAtualizacao = "Sistema";

        var tarefaAtualizadaDb = await _tarefaRepository.AtualizarTarefa(tarefa);
        return Result<TarefaEntity>.Ok(tarefaAtualizadaDb);
    }

    public async Task<Result> DeletarTarefaPorId(int id)
    {
        var deletado = await _tarefaRepository.DeletarTarefaPorId(id);
        if (!deletado)
            return Result.Falha("Nenhuma tarefa encontrada", TipoErro.NaoEncontrado);

        return Result.Ok();
    }
}
