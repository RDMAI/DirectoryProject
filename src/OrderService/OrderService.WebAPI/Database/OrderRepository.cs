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

    public async Task<Result<Order>> CreateAsync(Order entity, CancellationToken ct = default)
    {
        try
        {
            _dbContext.Orders.Add(entity);
            await _dbContext.SaveChangesAsync(ct);
            return entity;
        }
        catch (Exception ex)
        {
            return Error.Failure("create.failed", $"Failed to create order with id {entity.Id}");
        }
    }

    public async Task<Result<Order>> UpdateAsync(Order entity, CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);
            return entity;
        }
        catch (Exception ex)
        {
            return Error.Failure("update.failed", $"Failed to update order with id {entity.Id}");
        }
    }

    public async Task<Result<Order>> DeleteAsync(Order entity, CancellationToken ct = default)
    {
        try
        {
            _dbContext.Orders.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
            return entity;
        }
        catch (Exception ex)
        {
            return Error.Failure("delete.failed", $"Failed to delete order with id {entity.Id}");
        }
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
