using FluentValidation;
using Framework.Endpoints;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts.Events;
using OrderService.WebAPI.Database;
using OrderService.WebAPI.Domain;
using SharedKernel;

namespace OrderService.WebAPI.Features;

public sealed class DeleteOrder
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete(
                "/api/orders/{orderId:guid}",
                Handler);
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] Guid orderId,
        [FromServices] IOrderRepository repository,
        [FromServices] Bind<OrderCancelled, IPublishEndpoint> publishEndpoint,
        CancellationToken ct = default)
    {
        if (orderId == Guid.Empty)
            return EnvelopedResults.Error([ErrorHelper.General.ValueIsNullOrEmpty(nameof(orderId))]);

        var orderResult = await repository.GetByIdAsync(
            orderId: orderId,
            orderFilter: (orders) => orders
                .Where(o => o.Status == Order.OrderStatuses.Pending || o.Status == Order.OrderStatuses.Confirmed),
            ct: ct);
        if (orderResult.IsFailure)
            return EnvelopedResults.Error(orderResult.Errors);

        var entity = orderResult.Value;
        var cancelResult = entity.Cancel();
        if (cancelResult.IsFailure)
            return EnvelopedResults.Error(cancelResult.Errors);

        var updateResult = await repository.UpdateAsync(orderResult.Value, ct);
        if (updateResult.IsFailure)
            return EnvelopedResults.Error(updateResult.Errors);

        await publishEndpoint.Value.Publish(
            new OrderCancelled(orderId),
            publishContext => { publishContext.CorrelationId = orderId; },
            ct);

        return EnvelopedResults.Ok(orderResult.Value.Id);
    }
}