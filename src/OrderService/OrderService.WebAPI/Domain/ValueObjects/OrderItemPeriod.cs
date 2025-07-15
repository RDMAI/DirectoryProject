using SharedKernel;

namespace OrderService.WebAPI.Domain.ValueObjects;

public record OrderItemPeriod
{
    public DateTime StartDate { get; private init; }
    public DateTime EndDate { get; private init; }

    public static Result<OrderItemPeriod> Create(DateTime startDateTime, DateTime endDateTime)
    {
        if (startDateTime < DateTime.Today)
            return ErrorHelper.General.ValueIsInvalid(nameof(StartDate));

        if (endDateTime <= startDateTime)
            return ErrorHelper.General.ValueIsInvalid(nameof(EndDate));

        return new OrderItemPeriod(startDateTime, endDateTime);
    }

    private OrderItemPeriod(DateTime startDateTime, DateTime endDateTime)
    {
        StartDate = startDateTime;
        EndDate = endDateTime;
    }

    // EF Core
    private OrderItemPeriod() { }
}
