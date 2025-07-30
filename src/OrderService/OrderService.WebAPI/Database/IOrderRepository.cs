using OrderService.WebAPI.Domain;
using SharedKernel;

namespace OrderService.WebAPI.Database;

public interface IOrderRepository
{
    Task<Result<Order>> CreateAsync(Order entity, CancellationToken ct = default);
    Task<Result<Order>> UpdateAsync(Order entity, CancellationToken ct = default);
    Task<Result<Order>> DeleteAsync(Order entity, CancellationToken ct = default);
    Task<Result<Order>> GetByIdAsync(
        Guid orderId,
        Func<IQueryable<Order>, IQueryable<Order>>? orderFilter = null,
        CancellationToken ct = default);
}