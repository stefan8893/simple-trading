using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Extensions;

public static class ModelStateExtensions
{
    public static ActionResult ToCustomErrorResponse(this ModelStateDictionary modelStateDictionary)
    {
        var fieldErrors = modelStateDictionary
            .Where(x => x.Value is not null)
            .Where(x => x.Value!.ValidationState == ModelValidationState.Invalid)
            .Select(modelStateEntry => new FieldError
            {
                Identifier = modelStateEntry.Key,
                Reasons = modelStateEntry.Value!.Errors.Select(x => x.ErrorMessage).ToList()
            })
            .ToList();

        return new BadRequestObjectResult(new FieldErrorResponse
        {
            Errors = fieldErrors
        });
    }
}