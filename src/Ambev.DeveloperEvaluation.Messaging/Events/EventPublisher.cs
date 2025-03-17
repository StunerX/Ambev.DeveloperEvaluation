using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Messaging.Events;

public class EventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        Console.WriteLine($"Event Published: {JsonSerializer.Serialize(@event)}");
        return Task.CompletedTask;
    }
}