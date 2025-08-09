using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Generators.Tests;

public class SomeValidatorRequestModel
{
    public string? FooBar { get; set; } = string.Empty;
}

public class SomeValidatorResponseModel
{
};

public class SomeRequestModelValidator : AbstractValidator<SomeValidatorRequestModel>
{
    public SomeRequestModelValidator()
    {
        RuleFor(x => x.FooBar).NotNull();
    }
}

public class WithValidatorInteractor : IInteractor<SomeValidatorRequestModel, SomeValidatorResponseModel>
{
    public Task<SomeValidatorResponseModel> Execute(SomeValidatorRequestModel model)
    {
        return Task.FromResult(new SomeValidatorResponseModel());
    }
}

public class WithValidatorAndValidationResultInResponseModelInteractor : IInteractor<SomeValidatorRequestModel,
    OneOf<SomeValidatorResponseModel, ValidationResult>>
{
    public async Task<OneOf<SomeValidatorResponseModel, ValidationResult>> Execute(
        SomeValidatorRequestModel requestModel)
    {
        await Task.Yield();
        return new SomeValidatorResponseModel();
    }
}

public class InteractorWithValidatorTests
{
    [Fact]
    public async Task Response_model_is_transformed_when_a_validator_exists()
    {
        // arrange
        var validator = new SomeRequestModelValidator();

        IWithValidator proxy = new WithValidatorInteractorProxy(NullLogger<WithValidatorInteractorProxy>.Instance,
            new WithValidatorInteractor(), [validator]);

        // act
        var result = await proxy.Execute(new SomeValidatorRequestModel());

        // assert
        Assert.IsType<OneOf<SomeValidatorResponseModel, ValidationResult>>(result);
    }

    [Fact]
    public async Task Response_model_is_not_transformed_when_validation_result_is_included_in_OneOf()
    {
        // arrange
        var validator = new SomeRequestModelValidator();

        IWithValidatorAndValidationResultInResponseModel proxy =
            new WithValidatorAndValidationResultInResponseModelInteractorProxy(
                NullLogger<WithValidatorAndValidationResultInResponseModelInteractorProxy>.Instance,
                new WithValidatorAndValidationResultInResponseModelInteractor(), [validator]);

        // act
        var result = await proxy.Execute(new SomeValidatorRequestModel());

        // assert
        Assert.IsType<OneOf<SomeValidatorResponseModel, ValidationResult>>(result);
    }

    [Fact]
    public async Task Validation_is_performed_when_a_validator_exists()
    {
        // arrange
        var validator = new SomeRequestModelValidator();

        IWithValidator proxy = new WithValidatorInteractorProxy(NullLogger<WithValidatorInteractorProxy>.Instance,
            new WithValidatorInteractor(), [validator]);

        // act
        var result = await proxy.Execute(new SomeValidatorRequestModel {FooBar = null});

        // assert
        var validationResult = Assert.IsType<ValidationResult>(result.Value);
        var singleError = Assert.Single(validationResult.Errors);
        Assert.Equal("FooBar", singleError.PropertyName);
        Assert.Equal("'Foo Bar' must not be empty.", singleError.ErrorMessage);
    }
}