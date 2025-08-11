using JetBrains.Annotations;

namespace SimpleTrading.Domain.Generators.Tests;

[UsedImplicitly]
public static class TestSources
{
    public const string InteractorWithMissingInteractorSuffix =
        // lang=C#
        """
        using SimpleTrading.Domain.Infrastructure;
        using System.Threading;
        using System.Threading.Tasks;

        namespace SimpleTrading.Domain.Generators.Tests;

        public class GetFoobarRequestModel
        {
        }

        public class GetFoobarResponseModel
        {
        }

        public class GetFoobarInteractorWithoutProperSuffix : IInteractor<GetFoobarRequestModel, GetFoobarResponseModel>
        {
            public Task<GetFoobarResponseModel> Execute(GetFoobarRequestModel model, CancellationToken cancellationToken)
            {
                return Task.FromResult(new GetFoobarResponseModel());
            }
        }
        """;
}