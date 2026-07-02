using AgendaAutomatizada.Domain.Entities;

namespace AgendaAutomatizada.Domain.Interfaces;

public interface ITarefaRepository
{
    Task<TarefaEntity> CriarTarefa(TarefaEntity tarefa);
    Task<List<TarefaEntity>> ListarTarefas();
    Task<TarefaEntity?> ObterTarefaPorId(int id);
    Task<TarefaEntity> AtualizarTarefa(TarefaEntity tarefa);
    Task<bool> DeletarTarefaPorId(int id);
}
