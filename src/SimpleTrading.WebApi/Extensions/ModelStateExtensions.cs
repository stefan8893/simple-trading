using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Extensions;

public static class ModelStateExtensions
{
    public static ActionResult ToCustomErrorResponse(this ModelStateDictionary modelStateDictionary)
    {
        var reasons = modelStateDictionary
            .Where(x => x.Value is not null)
            .Where(x => x.Value!.ValidationState == ModelValidationState.Invalid)
            .SelectMany(modelStateEntry => modelStateEntry.Value!.Errors.Select(x => x.ErrorMessage))
            .ToList();

        return new BadRequestObjectResult(new ErrorResponse
        {
            Reasons = reasons
        });
    }
}