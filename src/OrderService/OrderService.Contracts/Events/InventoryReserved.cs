using OrderService.Contracts.DTOs;

namespace OrderService.Contracts.Events;

public record InventoryReserved(Guid OrderId, IEnumerable<ReservedBoxDTO> Reservations);