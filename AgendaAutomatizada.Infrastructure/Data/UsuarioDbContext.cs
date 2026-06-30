
namespace AgendaAutomatizada.Infrastructure.Data
{
    public class UsuarioDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options) : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);
            modelBuilder.Entity<Usuario>().Property(u => u.Nome).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Usuario>().Property(u => u.Email).IsUnique().IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Usuario>().Property(u => u.SenhaHash).IsRequired();
            modelBuilder.Entity<Usuario>().Property(u => u.Cpf).IsUnique().IsRequired().HasMaxLength(11);
            modelBuilder.Entity<Usuario>().Property(u => u.Telefone).IsRequired().HasMaxLength(15);
            modelBuilder.Entity<Usuario>().Property(u => u.DataCriacao).IsRequired();
            modelBuilder.Entity<Usuario>().Property(u => u.UsuarioAtualizacao).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Usuario>().Property(u => u.DataAtualizacao).IsRequired();
        }
    }
}