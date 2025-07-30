using OrderService.Contracts.DTOs;

namespace OrderService.Contracts.Requests;

public record CreateOrderRequest(
    Guid CustomerId,
    List<OrderItemDTO> Items);
