namespace AgendaAutomatizada.Api.DTOs.Responses;

public class UsuarioResponse
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}