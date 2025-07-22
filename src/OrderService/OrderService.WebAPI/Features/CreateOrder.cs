using Core.Validation;
using FluentValidation;
using Framework.Endpoints;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts.DTOs;
using OrderService.Contracts.Events;
using OrderService.Contracts.Requests;
using OrderService.WebAPI.Database;
using OrderService.WebAPI.Domain;
using OrderService.WebAPI.Domain.ValueObjects;
using SharedKernel;

namespace OrderService.WebAPI.Features;

public sealed class CreateOrder
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost(
                "/api/orders",
                Handler);
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] CreateOrderRequest request,
        [FromServices] IOrderRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        CancellationToken ct = default)
    {
        var validator = new CreateOrderRequestValidator();
        var validatorResult = await validator.ValidateAsync(request, ct);
        if (!validatorResult.IsValid)
        {
            return EnvelopedResults.Error(validatorResult.Errors
                .Select(e => Error.Deserialize(e.ErrorMessage))
                .ToList());
        }

        Guid orderId = Guid.NewGuid();

        List<OrderItem> orderItems = new(request.Items.Count());
        foreach (var item in request.Items)
        {
            var orderItemResult = OrderItem.Create(
                id: Guid.NewGuid(),
                orderId: orderId,
                boxSize: OrderItemBoxSize.Create(item.BoxSize).Value,
                period: OrderItemPeriod.Create(item.StartDate, item.EndDate).Value,
                quantity: OrderItemQuantity.Create(item.Quantity).Value);

            if (orderItemResult.IsFailure)
                return EnvelopedResults.Error(orderItemResult.Errors);

            orderItems.Add(orderItemResult.Value);
        }

        var orderResult = Order.Create(
            id: orderId,
            customerId: request.CustomerId,
            items: orderItems);

        if (orderResult.IsFailure)
            return EnvelopedResults.Error(orderResult.Errors);

        var createResult = await repository.CreateAsync(orderResult.Value, ct);
        if (createResult.IsFailure)
            return EnvelopedResults.Error(createResult.Errors);

        await publishEndpoint.Publish(
            new OrderCreated(
                OrderId: orderId,
                Items: request.Items,
                CustomerId: request.CustomerId),
            publishContext => { publishContext.CorrelationId = orderId; },
            ct);

        return EnvelopedResults.Ok(orderResult.Value.Id);
    }

    private class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(c => c.CustomerId)
                .Must(d => d != Guid.Empty)
                .WithError(ErrorHelper.General.ValueIsNullOrEmpty("CustomerId"));

            RuleFor(c => c.Items)
                .NotEmpty()
                .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Items"));

            RuleForEach(c => c.Items)
                .SetValidator(new OrderItemDTOValidator())
                .WithError(ErrorHelper.General.ValueIsInvalid("Items"));
        }
    }

    private class OrderItemDTOValidator : AbstractValidator<OrderItemDTO>
    {
        public OrderItemDTOValidator()
        {
            RuleFor(o => o.BoxSize)
                .MustBeValueObject(OrderItemBoxSize.Create);

            RuleFor(o => new { o.StartDate, o.EndDate })
                .Must(p => OrderItemPeriod.Create(p.StartDate, p.EndDate).IsSuccess)
                .WithError(ErrorHelper.General.ValueIsInvalid("Period (start or end date)"));

            RuleFor(o => o.Quantity)
                .Must(q => OrderItemQuantity.Create(q).IsSuccess)
                .WithError(ErrorHelper.General.ValueIsInvalid("Quantity"));
        }
    }
}