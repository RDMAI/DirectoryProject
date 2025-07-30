namespace InventoryService.Contracts.Events;

public record InventoryReleased(
    Guid OrderId,
    IEnumerable<Guid> ReleasedBoxIds);
