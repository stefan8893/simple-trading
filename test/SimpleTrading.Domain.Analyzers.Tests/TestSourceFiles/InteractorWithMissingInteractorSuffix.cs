using JetBrains.Annotations;
using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Analyzers.Tests.TestSourceFiles;

[UsedImplicitly]
public class InteractorWithMissingInteractorSuffix
{
    [UsedImplicitly]
    public class GetFoobarRequestModel
    {
    }

    [UsedImplicitly]
    public class GetFoobarResponseModel
    {
    }

    [UsedImplicitly]
    public class GetFoobarInteractorWithoutProperSuffix : IInteractor<GetFoobarRequestModel, GetFoobarResponseModel>
    {
        public Task<GetFoobarResponseModel> Execute(GetFoobarRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}