using Framework.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts.DTOs;
using OrderService.WebAPI.Database;
using OrderService.WebAPI.Domain;
using SharedKernel;

namespace OrderService.WebAPI.Features;

public sealed class GetOrderById
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(
                "/api/orders/{orderId:guid}",
                Handler);
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] Guid orderId,
        [FromServices] IOrderRepository repository,
        CancellationToken ct = default)
    {
        if (orderId == Guid.Empty)
            return EnvelopedResults.Error([ErrorHelper.General.ValueIsNullOrEmpty(nameof(orderId))]);

        var orderResult = await repository.GetByIdAsync(
            orderId: orderId,
            orderFilter: (orders) => orders.Include(o => o.Items),
            ct: ct);
        if (orderResult.IsFailure)
            return EnvelopedResults.Error(orderResult.Errors);

        var response = new OrderDTO(
            Id: orderResult.Value.Id,
            CustomerId: orderResult.Value.CustomerId,
            Status: GetStatusAsString(orderResult.Value.Status),
            Items: orderResult.Value.Items.Select(o => new OrderItemDTO(
                BoxSize: o.BoxSize.Value,
                Quantity: o.Quantity.Value,
                StartDate: o.Period.StartDate,
                EndDate: o.Period.EndDate)));

        return EnvelopedResults.Ok(response);
    }

    private static string GetStatusAsString(Order.OrderStatuses statusEnum) => statusEnum switch
    {
        Order.OrderStatuses.Pending => "Pending",
        Order.OrderStatuses.Confirmed => "Confirmed",
        Order.OrderStatuses.Rejected => "Rejected",
        Order.OrderStatuses.Canceled => "Canceled",
        _ => "Canceled",
    };
}