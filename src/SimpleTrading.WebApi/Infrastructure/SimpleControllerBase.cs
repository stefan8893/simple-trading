using System.Net.Mime;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.WebApi.Infrastructure;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
public class SimpleControllerBase : ControllerBase
{
    private SimpleProblemDetails SimpleProblemDetails =>
        HttpContext
            .RequestServices
            .GetRequiredService<SimpleProblemDetails>();

    protected ActionResult UnprocessableEntityResult(ValidationResult validationResult)
    {
        var details = SimpleProblemDetails.CreateUnprocessableEntityDetails(validationResult);
        return new UnprocessableEntityObjectResult(details);
    }

    protected ActionResult BadRequestResult(ValidationResult validationResult)
    {
        var details = SimpleProblemDetails.CreateBadRequestDetails(validationResult);
        return new BadRequestObjectResult(details);
    }

    protected ActionResult NotFoundResult(NotFound notFound)
    {
        var details = SimpleProblemDetails.CreateNotFoundDetails(notFound);
        return new NotFoundObjectResult(details);
    }

    protected ActionResult ConflictResult(Conflict conflict)
    {
        var details = SimpleProblemDetails.CreateConflictDetails(conflict);
        return new ConflictObjectResult(details);
    }
}