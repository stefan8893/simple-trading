using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Infrastructure;

public class SimpleProblemDetails(IHttpContextAccessor httpContextAccessor)
{
    private string Resource => httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty;

    public ProblemDetails CreateUnauthenticatedDetails()
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = SimpleTradingStrings.AuthenticationRequired,
            Status = StatusCodes.Status401Unauthorized,
            Instance = Resource
        };
    }

    public ValidationProblemDetails CreateUnprocessableEntityDetails(BadInput badInput)
    {
        return new ValidationProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2",
            Title = SimpleTradingStrings.OneOrMoreValidationErrors,
            Status = StatusCodes.Status422UnprocessableEntity,
            Errors = ToErrors(badInput.ValidationResult),
            Instance = Resource
        };
    }

    public ValidationProblemDetails CreateBadRequestDetails(ValidationResult validationResult)
    {
        return new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = SimpleTradingStrings.OneOrMoreValidationErrors,
            Status = StatusCodes.Status400BadRequest,
            Errors = ToErrors(validationResult),
            Instance = Resource
        };
    }

    public ProblemDetails CreateNotFoundDetails(NotFound? notFound = null)
    {
        var notFoundMessage = notFound?.ResourceType is null
            ? SimpleTradingStrings.NotFound
            : string.Format(SimpleTradingStrings.NotFoundNamed,
                SimpleTradingStrings.ResourceManager.GetString(notFound.ResourceType));

        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = SimpleTradingStrings.NotFound,
            Detail = notFoundMessage,
            Status = StatusCodes.Status404NotFound,
            Instance = Resource
        };
    }

    public ProblemDetails CreateConflictDetails(Conflict conflict)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            Title = SimpleTradingStrings.Conflict,
            Detail = conflict.Details,
            Status = StatusCodes.Status409Conflict,
            Instance = Resource
        };
    }

    private static Dictionary<string, string[]> ToErrors(ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(k => k.Key.FirstCharToLower(),
                v => v.Select(e => e.ErrorMessage).ToArray());
    }
}