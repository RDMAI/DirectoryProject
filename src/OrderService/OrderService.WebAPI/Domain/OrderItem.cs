using OrderService.WebAPI.Domain.ValueObjects;
using SharedKernel;

namespace OrderService.WebAPI.Domain;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public OrderItemBoxSize BoxSize { get; private set; }
    public OrderItemPeriod Period { get; private set; }
    public OrderItemQuantity Quantity { get; private set; }

    public static Result<OrderItem> Create(
        Guid id,
        Guid orderId,
        OrderItemBoxSize boxSize,
        OrderItemPeriod period,
        OrderItemQuantity quantity)
    {
        if (id == Guid.Empty)
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(Id));
        if (orderId == Guid.Empty)
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(OrderId));

        return new OrderItem(id, orderId, boxSize, period, quantity);
    }

    public void ChangePeriod(OrderItemPeriod period) => Period = period;

    public void ChangeQuantity(OrderItemQuantity newQuantity) => Quantity = newQuantity;

    private OrderItem(
        Guid id,
        Guid orderId,
        OrderItemBoxSize boxSize,
        OrderItemPeriod period,
        OrderItemQuantity quantity)
    {
        Id = id;
        OrderId = orderId;
        BoxSize = boxSize;
        Period = period;
        Quantity = quantity;
    }

    // ef core
    private OrderItem() { }
}
