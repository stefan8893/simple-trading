namespace SimpleTrading.Domain.Abstractions;

public interface IEntity
{
    public Guid Id { get; }
    public DateTime Created { get; }
}