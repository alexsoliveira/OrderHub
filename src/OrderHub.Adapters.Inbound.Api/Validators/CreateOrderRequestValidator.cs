using FluentValidation;
using OrderHub.Adapters.Inbound.Api.Models;

namespace OrderHub.Adapters.Inbound.Api.Validators;

/// <summary>
/// Validador para CreateOrderRequest - Valida requisição de criação de pedido
/// Implementa regras de validação através do FluentValidation
/// </summary>
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId é obrigatório")
            .Must(BeValidGuid)
            .WithMessage("CustomerId deve ser um GUID válido");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Pedido deve conter pelo menos 1 item")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("Lista de itens não pode estar vazia")
            .Must(items => items != null && items.Count <= 10)
            .WithMessage("Pedido não pode conter mais de 10 itens");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemRequestValidator());
    }

    private static bool BeValidGuid(string? value)
    {
        return !string.IsNullOrEmpty(value) && Guid.TryParse(value, out _);
    }
}

/// <summary>
/// Validador para CreateOrderItemRequest - Valida itens do pedido
/// </summary>
public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId é obrigatório")
            .Must(BeValidGuid)
            .WithMessage("ProductId deve ser um GUID válido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Preço unitário deve ser maior que 0");
    }

    private static bool BeValidGuid(string? value)
    {
        return !string.IsNullOrEmpty(value) && Guid.TryParse(value, out _);
    }
}
