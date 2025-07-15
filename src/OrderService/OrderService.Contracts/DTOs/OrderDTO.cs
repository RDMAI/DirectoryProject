namespace OrderService.Contracts.DTOs;

public record OrderDTO(
    Guid Id,
    Guid CustomerId,
    string Status,
    IEnumerable<OrderItemDTO> Items);
