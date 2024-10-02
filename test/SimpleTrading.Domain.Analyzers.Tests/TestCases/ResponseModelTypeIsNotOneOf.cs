using FluentValidation;
using SimpleTrading.Domain.Abstractions;

namespace SimpleTrading.Domain.Analyzers.Tests.TestCases;

public class ResponseModelTypeIsNotOneOf
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

    public class GetFoobarInteractor : IInteractor<GetFoobarRequestModel, string>
    {
        public ValueTask<string> Execute(GetFoobarRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}