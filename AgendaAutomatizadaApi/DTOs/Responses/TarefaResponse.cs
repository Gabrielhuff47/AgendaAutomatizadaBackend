namespace AgendaAutomatizada.Api.DTOs.Responses;

public class TarefaResponse
{
    public int IdTarefa { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public DateTime DataCriacao { get; set; }
}
