namespace SimpleTrading.Domain.Trading.UseCases.Shared;

public record ProfileResponseModel(Guid Id, string Name, string? Description, bool IsActive)
{
    public static ProfileResponseModel From(Profile profile)
    {
        return new ProfileResponseModel(profile.Id, profile.Name, profile.Description, profile.IsActive);
    }
}