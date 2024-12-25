namespace SimpleTrading.Domain.Infrastructure;

public interface IEntity
{
    public Guid Id { get; }
    public DateTime Created { get; }
}