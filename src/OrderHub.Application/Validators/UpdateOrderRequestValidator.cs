using FluentValidation;
using OrderHub.Application.DTOs;

namespace OrderHub.Application.Validators;

/// <summary>
/// Validador para UpdateOrderRequest usando FluentValidation
/// Define regras de negócio para atualização de pedido
/// </summary>
public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId é obrigatório");

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
