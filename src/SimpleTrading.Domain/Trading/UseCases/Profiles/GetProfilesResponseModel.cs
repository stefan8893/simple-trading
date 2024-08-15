namespace SimpleTrading.Domain.Trading.UseCases.Profiles;

public record GetProfilesResponseModel(Guid Id, string Name, string? Description, bool IsSelected);