using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.User;

public class UserSettings : IEntity
{
    public required string Culture { get; set; }
    public string? Language { get; set; }
    public required string TimeZone { get; set; }
    public required DateTime LastModified { get; set; }
    public required Guid Id { get; init; }
    public required DateTime Created { get; init; }
}