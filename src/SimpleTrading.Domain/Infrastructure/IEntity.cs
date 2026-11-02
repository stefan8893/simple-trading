namespace SimpleTrading.Domain.Infrastructure;

public interface IEntity
{
    Guid Id { get; }
    DateTime Created { get; }
}