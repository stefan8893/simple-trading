using System.Net.Mime;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.WebApi.Infrastructure;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.ProblemJson)]
public class SimpleControllerBase : ControllerBase
{
    private SimpleProblemDetails SimpleProblemDetails =>
        HttpContext
            .RequestServices
            .GetRequiredService<SimpleProblemDetails>();

    protected ActionResult UnprocessableEntityResult(ValidationResult validationResult)
    {
        var details = SimpleProblemDetails.CreateUnprocessableEntityDetails(validationResult);
        var result = new UnprocessableEntityObjectResult(details);
        result.ContentTypes.Add(MediaTypeNames.Application.ProblemJson);

        return result;
    }

    protected ActionResult BadRequestResult(ValidationResult validationResult)
    {
        var details = SimpleProblemDetails.CreateBadRequestDetails(validationResult);
        var result = new BadRequestObjectResult(details);
        result.ContentTypes.Add(MediaTypeNames.Application.ProblemJson);

        return result;
    }

    protected ActionResult NotFoundResult(NotFound notFound)
    {
        var details = SimpleProblemDetails.CreateNotFoundDetails(notFound);
        var result = new NotFoundObjectResult(details);
        result.ContentTypes.Add(MediaTypeNames.Application.ProblemJson);

        return result;
    }

    protected ActionResult ConflictResult(Conflict conflict)
    {
        var details = SimpleProblemDetails.CreateConflictDetails(conflict);
        var result = new ConflictObjectResult(details);
        result.ContentTypes.Add(MediaTypeNames.Application.ProblemJson);

        return result;
    }
}