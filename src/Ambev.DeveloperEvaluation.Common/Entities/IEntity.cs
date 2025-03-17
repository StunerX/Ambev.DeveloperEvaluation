namespace Ambev.DeveloperEvaluation.Common.Entities;

/// <summary>
/// Defines contract for basic properties for entities.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IEntity<out TKey> where TKey : struct
{
    TKey Id { get; }
}

/// <summary>
/// Defines contract for basic properties for entities with primary key type <see cref="Guid"/>.
/// </summary>
public interface IEntity : IEntity<Guid>
{
}