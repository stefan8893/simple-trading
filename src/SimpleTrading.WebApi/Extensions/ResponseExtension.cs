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
        var errorResponse = new ErrorResponse
        {
            FieldErrors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .Select(x => new FieldError
                {
                    Identifier = x.Key,
                    Messages = x.Select(e => e.ErrorMessage).ToList()
                })
                .ToList(),
            CommonErrors = []
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
            FieldErrors = [],
            CommonErrors = [errorMessage]
        };

        return new NotFoundObjectResult(errorResponse);
    }

    public static ActionResult ToActionResult(this BusinessError businessError)
    {
        var errorResponse = new ErrorResponse
        {
            FieldErrors = [],
            CommonErrors = [businessError.Reason]
        };

        return new UnprocessableEntityObjectResult(errorResponse);
    }
}