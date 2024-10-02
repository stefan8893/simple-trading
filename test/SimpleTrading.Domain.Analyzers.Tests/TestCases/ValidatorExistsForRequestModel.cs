using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Analyzers.Tests.TestCases;

public class ValidatorExistsForRequestModel
{
    public class GetFoobarRequestModel
    {
        public string? Candidate { get; set; }
    }

    public class GetFoobarRequestModelValidator : AbstractValidator<GetFoobarRequestModel>
    {
        public GetFoobarRequestModelValidator()
        {
            RuleFor(x => x.Candidate).NotNull();
        }
    }

    public class GetFoobarResponseModel
    {
    }

    public class GetFoobarInteractor : IInteractor<GetFoobarRequestModel, OneOf<GetFoobarResponseModel, NotFound>>
    {
        public Task<OneOf<GetFoobarResponseModel, NotFound>> Execute(GetFoobarRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}