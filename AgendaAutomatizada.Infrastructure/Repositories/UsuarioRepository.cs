using Microsoft.EntityFrameworkCore;
using AgendaAutomatizada.Domain.Entities;
using AgendaAutomatizada.Domain.Interfaces;
using AgendaAutomatizada.Infrastructure.Data;

namespace AgendaAutomatizada.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AgendaDbContext _context;

    public UsuarioRepository(AgendaDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioEntity> CriarUsuario(UsuarioEntity usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<UsuarioEntity?> ObterUsuarioPorEmail(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<UsuarioEntity?> ObterUsuarioPorCpf(string cpf)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf);
    }
}
