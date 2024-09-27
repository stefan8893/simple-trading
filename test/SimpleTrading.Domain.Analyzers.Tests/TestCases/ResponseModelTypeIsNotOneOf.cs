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

    public interface IGetFoobar : IInteractor<GetFoobarRequestModel, string>
    {
    
    }   
}