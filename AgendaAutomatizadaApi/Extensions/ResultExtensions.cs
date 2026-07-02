using System.Threading.Tasks;
using AgendaAutomatizada.Service.Shared;
using Microsoft.AspNetCore.Http;

public static class ResultExtensions
{
    private static int MapStatusCode(TipoErro tipoErro) => tipoErro switch
    {
        TipoErro.NaoEncontrado => 404,
        TipoErro.Conflito => 409,
        TipoErro.NaoAutorizado => 401,
        TipoErro.Invalido => 400,
        _ => 400
    };

    public static async Task<bool> SendErrorIfFailedAsync(
        this Result resultado, HttpContext httpContext, CancellationToken ct)
    {
        if (resultado.Sucesso) return false;

        httpContext.Response.StatusCode = MapStatusCode(resultado.TipoErro);
        await httpContext.Response.WriteAsJsonAsync(new { mensagem = resultado.Mensagem }, ct);
        return true;
    }

    public static async Task<bool> SendErrorIfFailedAsync<T>(
        this Result<T> resultado, HttpContext httpContext, CancellationToken ct)
    {
        if (resultado.Sucesso) return false;

        httpContext.Response.StatusCode = MapStatusCode(resultado.TipoErro);
        await httpContext.Response.WriteAsJsonAsync(new { mensagem = resultado.Mensagem }, ct);
        return true;
    }
}