using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Extensions;

public static class ValidationFailureExtensions
{
    public static string GetPropertyName(this ValidationFailure validationFailure)
    {
        return validationFailure.CustomState is CustomPropertyName custom
            ? custom.Name
            : validationFailure.PropertyName;
    }
}