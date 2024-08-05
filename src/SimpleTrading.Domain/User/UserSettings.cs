using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.User;

public class UserSettings
{
    public required Guid Id { get; init; }

    public required Guid SelectedProfileId { get; set; }

    public required Profile SelectedProfile { get; set; }

    public required string Culture { get; set; }

    public string? Language { get; set; }

    public required string TimeZone { get; set; }

    public required DateTime UpdatedAt { get; set; }
}