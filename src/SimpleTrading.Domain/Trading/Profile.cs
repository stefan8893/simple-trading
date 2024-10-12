using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading;

public class Profile : IEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSelected { get; set; }
    public required Guid Id { get; init; }
    public required DateTime Created { get; init; }
}