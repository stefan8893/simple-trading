using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Trading;

public class Profile : IEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSelected { get; set; }
    public required DateTime Created { get; init; }
}