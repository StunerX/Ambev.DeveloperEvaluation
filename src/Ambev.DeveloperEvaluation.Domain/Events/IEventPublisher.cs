namespace Ambev.DeveloperEvaluation.Domain.Events;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
}