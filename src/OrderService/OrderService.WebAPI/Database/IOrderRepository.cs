using OrderService.WebAPI.Domain;
using SharedKernel;

namespace OrderService.WebAPI.Database;

public interface IOrderRepository
{
    Task<UnitResult> CreateAsync(Order entity, CancellationToken ct = default);
    Task<UnitResult> UpdateAsync(Order entity, CancellationToken ct = default);
    Task<UnitResult> DeleteAsync(Order entity, CancellationToken ct = default);
    Task<Result<Order>> GetByIdAsync(
        Guid orderId,
        Func<IQueryable<Order>, IQueryable<Order>> orderFilter,
        CancellationToken ct = default);
}