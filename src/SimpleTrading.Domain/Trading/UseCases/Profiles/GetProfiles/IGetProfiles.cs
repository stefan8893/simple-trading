using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

public interface
    IGetProfiles : IInteractor<GetProfilesRequestModel, OneOf<IReadOnlyList<GetProfilesResponseModel>, BadInput>>
{
}