using AgendaAutomatizada.Api.DTOs.Requests;
using AgendaAutomatizada.Api.DTOs.Responses;
using AgendaAutomatizada.Domain.Entities;

namespace AgendaAutomatizadaApi.Mappers;

public static class TarefaMapper
{
    public static TarefaEntity ToEntity(this TarefaRequest request)
    {
        return new TarefaEntity
        {
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            Data = request.Data,
        };
    }

    public static TarefaResponse ToResponse(this TarefaEntity tarefa)
    {
        return new TarefaResponse
        {
            IdTarefa = tarefa.IdTarefa,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            Data = tarefa.Data,
            DataCriacao = tarefa.DataCriacao
        };
    }

    public static void UpdateEntity(this TarefaRequest request, TarefaEntity tarefa)
    {
        tarefa.Titulo = request.Titulo;
        tarefa.Descricao = request.Descricao;
        tarefa.Data = request.Data;
    }
}
