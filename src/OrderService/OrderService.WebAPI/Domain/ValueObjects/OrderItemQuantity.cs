using SharedKernel;

namespace OrderService.WebAPI.Domain.ValueObjects;

public record OrderItemQuantity
{
    public int Value { get; private init; }

    public static Result<OrderItemQuantity> Create(int value)
    {
        if (value < 0)
            return ErrorHelper.General.ValueIsInvalid("Quantity");

        return new OrderItemQuantity(value);
    }

    private OrderItemQuantity(int value)
    {
        Value = value;
    }

    // EF Core
    private OrderItemQuantity() { }
}
