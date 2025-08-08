using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Filter;

[UsedImplicitly]
public class ValidationFilter(IServiceProvider serviceProvider, SimpleProblemDetails simpleProblemDetails)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (!context.ActionArguments.TryGetValue(parameter.Name, out var argumentValue))
                continue;

            if (argumentValue is null)
                continue;

            var validators = GetValidatorsOrDefault(parameter.ParameterType);
            if (validators is null)
                continue;

            var validationContext = CreateValidationContext(parameter.ParameterType, argumentValue);

            foreach (var validator in validators)
            {
                if (validator is null)
                    continue;

                var validationResult =
                    await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                if (validationResult.IsValid)
                    continue;

                CreateBadRequestResponse(context, validationResult);
                return;
            }
        }

        await next();
    }

    private void CreateBadRequestResponse(ActionExecutingContext context, ValidationResult validationResult)
    {
        var problemDetails = simpleProblemDetails.CreateBadRequestDetails(validationResult);
        var result = new BadRequestObjectResult(problemDetails);
        context.Result = result;
    }

    private IEnumerable<IValidator?>? GetValidatorsOrDefault(Type type)
    {
        var validatorGenericType = typeof(IValidator<>).MakeGenericType(type);
        var validatorsEnumerable = typeof(IEnumerable<>).MakeGenericType(validatorGenericType);

        return serviceProvider.GetService(validatorsEnumerable) as IEnumerable<IValidator>;
    }

    private static IValidationContext? CreateValidationContext(Type parameter, object argumentValue)
    {
        var validationContextType = typeof(ValidationContext<>).MakeGenericType(parameter);

        return Activator.CreateInstance(validationContextType, argumentValue) as IValidationContext;
    }
}