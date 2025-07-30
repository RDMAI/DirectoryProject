namespace OrderService.Contracts.Events;

public record InventoryUnavailable(Guid OrderId, string Reason);
