namespace InventoryService.WepAPI.Domain;

public class Reservation
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid BoxId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public Reservation(
        Guid id,
        Guid orderId,
        Guid boxId,
        DateTime startDate,
        DateTime endDate)
    {
        Id = id;
        OrderId = orderId;
        BoxId = boxId;
        StartDate = startDate;
        EndDate = endDate;
    }

    // EF Core
    private Reservation() { }
}
