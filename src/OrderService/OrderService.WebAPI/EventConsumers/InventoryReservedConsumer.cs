using InventoryService.Contracts.Events;
using MassTransit;
using OrderService.WebAPI.Database;

namespace OrderService.WebAPI.EventConsumers;

public class InventoryReservedConsumer : IConsumer<InventoryReserved>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<InventoryReservedConsumer> _logger;

    public InventoryReservedConsumer(
        IOrderRepository orderRepository,
        ILogger<InventoryReservedConsumer> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReserved> context)
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
        entity.Confirm();

        var updateResult = await _orderRepository.UpdateAsync(entity, context.CancellationToken);
        if (updateResult.IsFailure)
            _logger.LogError(updateResult.ToString());
    }
}
