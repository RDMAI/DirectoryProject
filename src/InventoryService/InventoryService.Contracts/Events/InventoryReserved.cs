namespace InventoryService.Contracts.Events;

public record InventoryReserved(
    Guid OrderId,
    IEnumerable<Guid> ReservedBoxIds);
