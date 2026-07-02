namespace AgendaAutomatizada.Domain.Entities;

public class UsuarioEntity
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public string UsuarioAtualizacao { get; set; } = string.Empty;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}
