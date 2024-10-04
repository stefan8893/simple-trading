using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Analyzers.Tests.TestSourceFiles;

[UsedImplicitly]
public class ResponseModelTypeIsNotOneOf
{
    [UsedImplicitly]
    public class GetFoobarRequestModel
    {
        public string? Candidate { get; set; }
    }

    [UsedImplicitly]
    public class GetFoobarRequestModelValidator : AbstractValidator<GetFoobarRequestModel>
    {
        public GetFoobarRequestModelValidator()
        {
            RuleFor(x => x.Candidate).NotNull();
        }
    }

    [UsedImplicitly]
    public class GetFoobarResponseModel
    {
    }

    [UsedImplicitly]
    public class GetFoobarInteractor : IInteractor<GetFoobarRequestModel, string>
    {
        public Task<string> Execute(GetFoobarRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}