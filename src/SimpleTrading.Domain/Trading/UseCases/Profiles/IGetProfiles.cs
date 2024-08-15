using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles;

public interface
    IGetProfiles : IInteractor<GetProfilesRequestModel, OneOf<IReadOnlyList<GetProfilesResponseModel>, BadInput>>
{
}