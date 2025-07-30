namespace InventoryService.Contracts.Events;

public record InventoryUnavaliable(
    Guid OrderId,
    string Reason);
