using InventoryService.Contracts.Events;
using InventoryService.WepAPI.ReservationManagement;
using MassTransit;
using OrderService.Contracts.Events;

namespace InventoryService.WepAPI.EventConsumers;

public class OrderCancelledConsumer : IConsumer<OrderCancelled>
{
    private readonly IReservationService _reservationService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(
        IReservationService reservationService,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderCancelledConsumer> logger)
    {
        _reservationService = reservationService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelled> context)
    {
        var releaseResult = await _reservationService.ReleaseAsync(
            context.Message.OrderId,
            context.CancellationToken);

        if (releaseResult.IsFailure)
        {
            _logger.LogError(releaseResult.ToString());
            return;
        }

        await _publishEndpoint.Publish(
            new InventoryReleased(
                context.Message.OrderId,
                releaseResult.Value),
            publishContext => { publishContext.CorrelationId = context.Message.OrderId; },
            context.CancellationToken);
    }
}
