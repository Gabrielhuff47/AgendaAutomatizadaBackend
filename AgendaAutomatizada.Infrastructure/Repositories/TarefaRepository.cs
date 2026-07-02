using Microsoft.EntityFrameworkCore;
using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Infrastructure.Data;

namespace AgendaAutomatizada.Infrastructure.Repositories;

public class TarefaRepository : ITarefaRepository
{
    private readonly AgendaDbContext _context;

    public TarefaRepository(AgendaDbContext context)
    {
        _context = context;
    }

    public async Task<TarefaEntity> CriarTarefa(TarefaEntity tarefa)
    {
        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();
        return tarefa;
    }

    public async Task<List<TarefaEntity>> ListarTarefas()
    {
        return await _context.Tarefas
            .OrderBy(t => t.Data)
            .ToListAsync();
    }

    public async Task<TarefaEntity?> ObterTarefaPorId(int id)
    {
        return await _context.Tarefas.FirstOrDefaultAsync(t => t.IdTarefa == id);
    }

    public async Task<TarefaEntity> AtualizarTarefa(TarefaEntity tarefa)
    {
        _context.Tarefas.Update(tarefa);
        await _context.SaveChangesAsync();
        return tarefa;
    }

    public async Task<bool> DeletarTarefaPorId(int id)
    {
        var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.IdTarefa == id);
        if (tarefa is null)
            return false;

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
        return true;
    }
}
