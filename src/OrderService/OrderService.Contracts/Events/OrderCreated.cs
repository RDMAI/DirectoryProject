using OrderService.Contracts.DTOs;

namespace OrderService.Contracts.Events;

public record OrderCreated(Guid OrderId, IEnumerable<OrderItemDTO> Items, Guid CustomerId);
