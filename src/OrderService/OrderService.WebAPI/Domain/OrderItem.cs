using SharedKernel;

namespace OrderService.WebAPI.Domain;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string BoxSize { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Quantity { get; private set; }

    public static Result<OrderItem> Create(
        Guid id,
        Guid orderId,
        string boxSize,
        DateTime startDate,
        DateTime endDate,
        int quantity)
    {
        if (id == Guid.Empty)
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(Id));
        if (orderId == Guid.Empty)
            return ErrorHelper.General.ValueIsNullOrEmpty(nameof(OrderId));
        if (string.IsNullOrEmpty(boxSize))
            return ErrorHelper.General.ValueIsInvalid(nameof(BoxSize));

        var entity = new OrderItem(id, orderId, boxSize);

        var changeStartDateResult = entity.ChangeStartDate(startDate);
        if (changeStartDateResult.IsFailure) return changeStartDateResult.Errors;

        var changeEndDateResult = entity.ChangeEndDate(endDate);
        if (changeEndDateResult.IsFailure) return changeEndDateResult.Errors;

        var changeQuantityResult = entity.ChangeQuantity(quantity);
        if (changeQuantityResult.IsFailure) return changeQuantityResult.Errors;

        return entity;
    }

    public UnitResult ChangeStartDate(DateTime newStartDateTime)
    {
        if (newStartDateTime < DateTime.Today)
            return ErrorHelper.General.ValueIsInvalid(nameof(StartDate));

        StartDate = newStartDateTime;

        return UnitResult.Success();
    }

    public UnitResult ChangeEndDate(DateTime newEndDateTime)
    {
        if (newEndDateTime <= StartDate)
            return ErrorHelper.General.ValueIsInvalid(nameof(EndDate));

        EndDate = newEndDateTime;

        return UnitResult.Success();
    }

    public UnitResult ChangeQuantity(int newQuantity)
    {
        if (newQuantity < 0)
            return ErrorHelper.General.ValueIsInvalid(nameof(Quantity));

        Quantity = newQuantity;

        return UnitResult.Success();
    }

    private OrderItem(
        Guid id,
        Guid orderId,
        string boxSize)
    {
        Id = id;
        OrderId = orderId;
        BoxSize = boxSize;
    }

    // ef core
    private OrderItem() { }
}
