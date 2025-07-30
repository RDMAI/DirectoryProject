using InventoryService.WepAPI.Database;
using InventoryService.WepAPI.Domain;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts.DTOs;
using SharedKernel;

namespace InventoryService.WepAPI.ReservationManagement;

public class ReservationService : IReservationService
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(
        ApplicationDBContext context,
        ILogger<ReservationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<Guid>>> TryReserveAsync(
        Guid orderId,
        IEnumerable<OrderItemDTO> positions,
        CancellationToken ct = default)
    {
        List<Guid> tempReservedBoxes = new(positions.Count());

        var transaction = await _context.Database.BeginTransactionAsync(ct);
        foreach (var p in positions)
        {
            var box = await (
                from b in _context.Boxes
                join res in _context.Reservations on b.Id equals res.BoxId into reservationGroup
                from r in reservationGroup.DefaultIfEmpty() // left join
                where tempReservedBoxes.Contains(b.Id) == false
                    && p.BoxSize == b.Size // required size
                    && (r == null // not reserved
                        || (r != null && p.StartDate > r.EndDate && r.StartDate > p.EndDate)) // reserved, no period overlaping
                select b)
                .FirstOrDefaultAsync(ct);
            if (box is null)
            {
                await transaction.RollbackAsync();
                return Error.Failure(
                    "no.box.avaliable",
                    $"Could not find avaliable box of size {p.BoxSize} to dates {p.StartDate} - {p.EndDate}");
            }

            tempReservedBoxes.Add(box.Id);

            _context.Reservations.Add(new Reservation(
                id: Guid.NewGuid(),
                orderId: orderId,
                boxId: box.Id,
                startDate: p.StartDate,
                endDate: p.EndDate));
        }

        await _context.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        _logger.LogInformation("Boxes for order with id {orderId} were reserved", orderId);

        return tempReservedBoxes;
    }

    /// <summary>
    /// Deletes all related reservations, activates reserved boxes.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="ct"></param>
    /// <returns>IEnumerable of freed box ids.</returns>
    public async Task<Result<IEnumerable<Guid>>> ReleaseAsync(
        Guid orderId,
        CancellationToken ct = default)
    {
        var boxIds = await _context.Reservations
            .Where(r => r.OrderId == orderId)
            .Select(r => r.BoxId)
            .ToListAsync(ct);
        if (boxIds is null || boxIds.Count == 0)
            return ErrorHelper.General.NotFound(orderId);

        var deletedReservationCount = await _context.Reservations
            .Where(r => r.OrderId == orderId)
            .ExecuteDeleteAsync(ct);
        if (deletedReservationCount == 0)
            return ErrorHelper.General.NotFound(orderId);

        _logger.LogInformation("Boxes in order with id {orderId} were released", orderId);

        return boxIds;
    }
}
