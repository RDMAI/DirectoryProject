using InventoryService.Contracts.Events;
using InventoryService.WepAPI.ReservationManagement;
using MassTransit;
using OrderService.Contracts.Events;

namespace InventoryService.WepAPI.EventConsumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly IReservationService _reservationService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        IReservationService reservationService,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderCreatedConsumer> logger)
    {
        _reservationService = reservationService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var reservationResult = await _reservationService.TryReserveAsync(
            context.Message.OrderId,
            context.Message.Items,
            context.CancellationToken);

        if (reservationResult.IsFailure)
        {
            await _publishEndpoint.Publish(
                new InventoryUnavaliable(
                    context.Message.OrderId,
                    reservationResult.ToString()),
                publishContext => { publishContext.CorrelationId = context.Message.OrderId; },
                context.CancellationToken);
        }
        else
        {
            await _publishEndpoint.Publish(
                new InventoryReserved(
                    context.Message.OrderId,
                    reservationResult.Value),
                publishContext => { publishContext.CorrelationId = context.Message.OrderId; },
                context.CancellationToken);
        }
    }
}
