using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.User;

public class UserSettings : IEntity
{
    public required Guid Id { get; init; }

    public required string Culture { get; set; }

    public string? Language { get; set; }

    public required string TimeZone { get; set; }

    public required DateTime LastModified { get; set; }

    public required DateTime Created { get; init; }
}