using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Extensions;

public static class ResponseExtension
{
    public static ActionResult ToActionResult(this BadInput badInput)
    {
        return badInput.ValidationResult.ToActionResult();
    }

    public static ActionResult ToActionResult(this ValidationResult validationResult)
    {
        var errorResponse = new FieldErrorResponse
        {
            Errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .Select(x => new FieldError
                {
                    Identifier = x.Key,
                    Messages = x.Select(e => e.ErrorMessage).ToList()
                })
                .ToList()
        };

        return new BadRequestObjectResult(errorResponse);
    }

    public static ActionResult ToActionResult(this NotFound notFound)
    {
        var errorMessage = notFound.ResourceType is null
            ? SimpleTradingStrings.NotFound
            : string.Format(SimpleTradingStrings.NotFoundNamed,
                SimpleTradingStrings.ResourceManager.GetString(notFound.ResourceType));

        var errorResponse = new ErrorResponse
        {
            Messages = [errorMessage]
        };

        return new NotFoundObjectResult(errorResponse);
    }

    public static ActionResult ToActionResult(this BusinessError businessError)
    {
        var errorResponse = new ErrorResponse
        {
            Messages = [businessError.Details]
        };

        return new UnprocessableEntityObjectResult(errorResponse);
    }
}