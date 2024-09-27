using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Analyzers.Tests.TestCases;

public class ValidatorAndBadInputCaseExists
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

    public interface IGetFoobar : IInteractor<GetFoobarRequestModel, OneOf<GetFoobarResponseModel, BadInput, NotFound>>
    {
    }
}