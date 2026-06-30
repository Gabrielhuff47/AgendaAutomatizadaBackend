namespace AgendaAutomatizada.Api.DTOs.Requests;


public class UsuarioRequest
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Cpf { get; set; }
    public string Telefone { get; set; }
}