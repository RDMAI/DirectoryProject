using SharedKernel;

namespace OrderService.WebAPI.Domain.ValueObjects;

public record OrderItemBoxSize
{
    public static readonly string[] PossibleValues = [ "S", "M", "L" ];

    public string Value { get; private init; }

    public static Result<OrderItemBoxSize> Create(string value)
    {
        if (PossibleValues.Contains(value) == false)
            return ErrorHelper.General.ValueIsInvalid("BoxSize");

        return new OrderItemBoxSize(value);
    }

    private OrderItemBoxSize(string value)
    {
        Value = value;
    }

    // EF Core
    private OrderItemBoxSize() { }
}
