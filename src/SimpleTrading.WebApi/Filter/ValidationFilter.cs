using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleTrading.WebApi.Extensions;

namespace SimpleTrading.WebApi.Filter;

[UsedImplicitly]
public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (!context.ActionArguments.TryGetValue(parameter.Name, out var argumentValue))
                continue;

            if (argumentValue is null)
                continue;

            var validator = GetValidatorOrDefault(parameter.ParameterType);
            if (validator is null)
                continue;

            var validationContext = CreateValidationContext(parameter.ParameterType, argumentValue);
            var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (validationResult.IsValid)
                continue;

            context.Result = validationResult.ToActionResult();
            return;
        }

        await next();
    }

    private IValidator? GetValidatorOrDefault(Type type)
    {
        var validatorGenericType = typeof(IValidator<>).MakeGenericType(type);

        return serviceProvider.GetService(validatorGenericType) as IValidator;
    }

    private static IValidationContext? CreateValidationContext(Type parameter, object argumentValue)
    {
        var validationContextType = typeof(ValidationContext<>).MakeGenericType(parameter);

        return Activator.CreateInstance(validationContextType, argumentValue) as IValidationContext;
    }
}