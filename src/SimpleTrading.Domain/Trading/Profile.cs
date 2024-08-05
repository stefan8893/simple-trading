namespace SimpleTrading.Domain.Trading;

public class Profile
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public string? Description { get; set; } = "";

    public required DateTime CreatedAt { get; init; }
}