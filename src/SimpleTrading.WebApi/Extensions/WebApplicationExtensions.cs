using Microsoft.AspNetCore.Localization;
using SimpleTrading.Domain;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.WebApi.Extensions;

public static class WebApplicationExtensions
{
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
        var userSettings = await dbContext.GetOrDefault();

        if (userSettings is null)
            return new ProviderCultureResult(Constants.DefaultCulture.Name);

        if (userSettings.Language is null)
            return new ProviderCultureResult(userSettings.Culture);

        return new ProviderCultureResult(userSettings.Culture,
            Constants.SupportedCultures
                .Select(x => x.Name)
                .FirstOrDefault(x => x.StartsWith(userSettings.Language))
            ?? Constants.DefaultCulture.Name);
    }
}