using OrderService.Contracts.DTOs;
using SharedKernel;

namespace InventoryService.WepAPI.ReservationManagement;
public interface IReservationService
{
    Task<Result<IEnumerable<Guid>>> ReleaseAsync(
        Guid orderId,
        CancellationToken ct = default);

    Task<Result<IEnumerable<Guid>>> TryReserveAsync(
        Guid orderId,
        IEnumerable<OrderItemDTO> positions,
        CancellationToken ct = default);
}