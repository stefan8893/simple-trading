using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Analyzers.Tests.TestSourceFiles;

[UsedImplicitly]
public class ValidatorAndBadInputCaseExists
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
    public class
        GetFoobarInteractor : IInteractor<GetFoobarRequestModel, OneOf<GetFoobarResponseModel, ValidationResult, NotFound>>
    {
        public Task<OneOf<GetFoobarResponseModel, ValidationResult, NotFound>> Execute(GetFoobarRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}