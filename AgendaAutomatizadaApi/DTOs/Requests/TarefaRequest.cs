namespace AgendaAutomatizada.Api.DTOs.Requests;

public class TarefaRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
}
