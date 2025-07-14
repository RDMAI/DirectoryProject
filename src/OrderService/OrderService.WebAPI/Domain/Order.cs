using SharedKernel;

namespace OrderService.WebAPI.Domain;

public class Order
{
    public enum OrderStatuses
    {
        Pending = 10,
        Confirmed = 20,
        Rejected = 30,
        Canceled = 31,
        Finished = 39,
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatuses Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    private List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public static Result<Order> Create(
        Guid id,
        Guid customerId,
        List<OrderItem> items)
    {
        if (id == Guid.Empty)
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(Id));
        if (customerId == Guid.Empty)
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(CustomerId));
        if (items.Count == 0)
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(Items));

        return new Order(id, customerId, items);
    }

    public UnitResult Confirm()
    {
        if (Status != OrderStatuses.Pending)
            return ErrorHelper.General.MethodNotApplicable("Order status is invalid for this command");

        Status = OrderStatuses.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success();
    }

    public UnitResult Reject()
    {
        Status = OrderStatuses.Rejected;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success();
    }

    public UnitResult Cancel()
    {
        if (Status != OrderStatuses.Pending && Status != OrderStatuses.Confirmed)
            return ErrorHelper.General.MethodNotApplicable("Order status is invalid for this command");

        Status = OrderStatuses.Canceled;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success();
    }

    private Order(
        Guid id,
        Guid customerId,
        List<OrderItem> items)
    {
        Id = id;
        CustomerId = customerId;
        _items = items;
        Status = OrderStatuses.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // ef core
    private Order() { }
}
