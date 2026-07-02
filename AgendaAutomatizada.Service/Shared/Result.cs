// AgendaAutomatizada.Service/Shared/Result.cs
namespace AgendaAutomatizada.Service.Shared;

public enum TipoErro
{
    Nenhum,
    NaoEncontrado,
    Conflito,
    NaoAutorizado,
    Invalido
}

public class Result
{
    public bool Sucesso { get; protected init; }
    public string Mensagem { get; protected init; } = string.Empty;
    public TipoErro TipoErro { get; protected init; } = TipoErro.Nenhum;

    protected Result() { }

    public static Result Ok()
        => new() { Sucesso = true };

    public static Result Falha(string mensagem, TipoErro tipoErro = TipoErro.Invalido)
        => new() { Sucesso = false, Mensagem = mensagem, TipoErro = tipoErro };
}

public class Result<T> : Result
{
    public T? Dados { get; protected init; }

    public static Result<T> Ok(T dados)
        => new() { Sucesso = true, Dados = dados };

    public static new Result<T> Falha(string mensagem, TipoErro tipoErro = TipoErro.Invalido)
        => new() { Sucesso = false, Mensagem = mensagem, TipoErro = tipoErro };
}