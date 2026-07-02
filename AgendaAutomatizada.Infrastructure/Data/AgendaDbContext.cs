using Microsoft.EntityFrameworkCore;
using AgendaAutomatizada.Domain.Entities;

namespace AgendaAutomatizada.Infrastructure.Data
{
    public class AgendaDbContext : DbContext
    {
        public DbSet<UsuarioEntity> Usuarios { get; set; }

        public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioEntity>(entity =>
            {
                entity.ToTable("Usuarios");

                entity.HasKey(u => u.IdUsuario);

                entity.Property(u => u.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.SenhaHash)
                    .IsRequired();

                entity.Property(u => u.Cpf)
                    .IsRequired()
                    .HasMaxLength(11);

                entity.HasIndex(u => u.Cpf)
                    .IsUnique();

                entity.Property(u => u.Telefone)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(u => u.DataCriacao)
                    .IsRequired();

                entity.Property(u => u.UsuarioAtualizacao)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.DataAtualizacao)
                    .IsRequired();
            });
        }
    }
}