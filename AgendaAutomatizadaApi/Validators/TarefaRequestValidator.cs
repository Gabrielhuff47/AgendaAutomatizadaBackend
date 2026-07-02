using FastEndpoints;
using FluentValidation;
using AgendaAutomatizada.Api.DTOs.Requests;

namespace AgendaAutomatizadaApi.Validators;

public class TarefaRequestValidator : Validator<TarefaRequest>
{
    public TarefaRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(100).WithMessage("Título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória.")
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data é obrigatória.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Data não pode ser no passado.");
    }
}
