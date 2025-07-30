namespace OrderService.Contracts.Messaging;

public record RabbitMQOptions(
    string Host,
    string Username,
    string Password,
    int RetryLimit,
    int RetryMinIntervalSeconds,
    int RetryMaxIntervalSeconds,
    int RetryDeltaSeconds)
{
    public const string SECTION_NAME = "RabbitMQ";
}
