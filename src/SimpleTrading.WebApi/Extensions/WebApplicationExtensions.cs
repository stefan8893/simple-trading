using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Localization;
using SimpleTrading.Domain;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.User.DataAccess;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// In cases where the request path cannot be mapped to an endpoint.<br/>
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
                context.Response.ContentType = "application/json; charset=utf-8";
                var jsonResponse = JsonSerializer.Serialize(new ErrorResponse
                    {Reasons = [SimpleTradingStrings.NotFound]});
                await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
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