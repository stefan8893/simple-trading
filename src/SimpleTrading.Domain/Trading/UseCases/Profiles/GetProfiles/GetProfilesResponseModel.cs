namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

public record GetProfilesResponseModel(Guid Id, string Name, string? Description, bool IsActive);