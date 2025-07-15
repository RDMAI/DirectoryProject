namespace OrderService.Contracts.DTOs;

public record OrderItemDTO(
    string BoxSize,
    int Quantity,
    DateTime StartDate,
    DateTime EndDate);
