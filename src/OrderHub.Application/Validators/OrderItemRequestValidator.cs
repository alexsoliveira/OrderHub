using FluentValidation;
using OrderHub.Application.DTOs;

namespace OrderHub.Application.Validators;

/// <summary>
/// Validador para OrderItemRequest usando FluentValidation
/// Define regras de negócio para itens de pedido
/// </summary>
public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId é obrigatório");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que 0");

        RuleFor(x => x.Quantity)
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantidade não pode exceder 1000 unidades");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Preço unitário deve ser maior que 0");

        RuleFor(x => x.UnitPrice)
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Preço unitário não pode exceder 999999,99");
    }
}
