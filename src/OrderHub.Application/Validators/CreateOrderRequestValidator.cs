using FluentValidation;
using OrderHub.Application.DTOs;

namespace OrderHub.Application.Validators;

/// <summary>
/// Validador para CreateOrderRequest usando FluentValidation
/// Define regras de negócio para criação de novo pedido
/// </summary>
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId é obrigatório");

        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Items não pode ser nulo")
            .NotEmpty()
            .WithMessage("Pedido deve conter no mínimo 1 item");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemRequestValidator())
            .When(x => x.Items != null);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Descrição não pode exceder 500 caracteres");
    }
}
