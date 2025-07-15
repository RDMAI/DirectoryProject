using Microsoft.EntityFrameworkCore;
using OrderService.WebAPI.Domain;
using SharedKernel;

namespace OrderService.WebAPI.Database;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDBContext _dbContext;

    public OrderRepository(ApplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UnitResult> CreateAsync(Order entity, CancellationToken ct = default)
    {
        _dbContext.Orders.Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        return UnitResult.Success();
    }

    public async Task<UnitResult> UpdateAsync(Order entity, CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);

        return UnitResult.Success();
    }

    public async Task<UnitResult> DeleteAsync(Order entity, CancellationToken ct = default)
    {
        _dbContext.Orders.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);

        return UnitResult.Success();
    }

    public async Task<Result<Order>> GetByIdAsync(
        Guid orderId,
        Func<IQueryable<Order>, IQueryable<Order>> orderFilter,
        CancellationToken ct = default)
    {
        IQueryable<Order> orderQuery = _dbContext.Orders;
        if (orderFilter is not null)
            orderQuery = orderFilter(orderQuery);

        var entity = await orderQuery.FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (entity is null)
            return ErrorHelper.General.NotFound(orderId);

        return entity;
    }
}
