using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Localization;
using SimpleTrading.Domain;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    ///     In cases where the request path cannot be mapped to an endpoint.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseNotFoundMiddleware(this WebApplication app)
    {
        return app.Use(async (context, next) =>
        {
            await next();

            if (context.Response is {StatusCode: 404, HasStarted: false})
            {
                var simpleProblemDetails = context.RequestServices.GetRequiredService<SimpleProblemDetails>();

                var notFoundProblemDetails = simpleProblemDetails.CreateNotFoundDetails();
                context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
                var jsonResponse = JsonSerializer.Serialize(notFoundProblemDetails);

                await context.Response.WriteAsync(jsonResponse);
            }
        });
    }

    public static IApplicationBuilder Use401ResponseBodyProblemDetailsMiddleware(this WebApplication app)
    {
        return app.Use(async (context, next) =>
        {
            await next();

            if (context.Response is {StatusCode: StatusCodes.Status401Unauthorized, HasStarted: false})
            {
                var simpleProblemDetails = context.RequestServices.GetRequiredService<SimpleProblemDetails>();

                var problemDetails = simpleProblemDetails.CreateUnauthenticatedDetails();
                var jsonResponse = JsonSerializer.Serialize(problemDetails);

                context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
                await context.Response.WriteAsync(jsonResponse);
            }
        });
    }

    public static IApplicationBuilder UseRequestLocalization(this WebApplication app)
    {
        return app.UseRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(Constants.DefaultCulture, Constants.DefaultCulture);
            options.SupportedCultures = Constants.SupportedCultures.ToList();
            options.SupportedUICultures = Constants.SupportedCultures.ToList();
            options.SetDefaultCulture(Constants.DefaultCulture.Name);

            options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(GetCurrentRequestCulture));
        });
    }

    private static async Task<ProviderCultureResult?> GetCurrentRequestCulture(HttpContext context)
    {
        var dbContext = context.RequestServices.GetRequiredService<IUserSettingsRepository>();
        var userSettings = await dbContext.GetUserSettingsOrDefault();

        if (userSettings is null)
            return new ProviderCultureResult(Constants.DefaultCulture.Name);

        if (userSettings.Language is null)
            return new ProviderCultureResult(userSettings.Culture);

        return new ProviderCultureResult(userSettings.Culture,
            Constants.SupportedCultures
                .Select(x => x.Name)
                .FirstOrDefault(x => x.StartsWith(userSettings.Language, StringComparison.OrdinalIgnoreCase))
            ?? Constants.DefaultCulture.Name);
    }
}