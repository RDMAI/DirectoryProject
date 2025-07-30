using InventoryService.Contracts.Events;
using MassTransit;
using OrderService.WebAPI.Database;

namespace OrderService.WebAPI.EventConsumers;

public class InventoryUnavaliableConsumer : IConsumer<InventoryUnavaliable>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<InventoryUnavaliableConsumer> _logger;

    public InventoryUnavaliableConsumer(
        IOrderRepository orderRepository,
        ILogger<InventoryUnavaliableConsumer> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryUnavaliable> context)
    {
        var orderResult = await _orderRepository.GetByIdAsync(
            orderId: context.Message.OrderId,
            ct: context.CancellationToken);
        if (orderResult.IsFailure)
        {
            _logger.LogError(orderResult.ToString());
            return;
        }

        var entity = orderResult.Value;
        entity.Reject();

        var updateResult = await _orderRepository.UpdateAsync(entity, context.CancellationToken);
        if (updateResult.IsFailure)
            _logger.LogError(updateResult.ToString());
    }
}
