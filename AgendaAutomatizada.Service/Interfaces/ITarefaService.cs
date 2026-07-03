using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Service.Shared;

namespace AgendaAutomatizada.Service.Interfaces;

public interface ITarefaService
{
    Task<Result<TarefaEntity>> CriarTarefa(TarefaEntity tarefa);
    Task<Result<List<TarefaEntity>>> ListarTarefas();
    Task<Result<TarefaEntity>> ObterTarefaPorId(int id);
    Task<Result<TarefaEntity>> AtualizarTarefaPorId(int id, TarefaEntity tarefaAtualizada);
    Task<Result> DeletarTarefaPorId(int id);
}
