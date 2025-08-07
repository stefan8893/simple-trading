using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features;

[Route("")]
public class HomeController(IHostEnvironment hostEnvironment, IConfiguration configuration) : SimpleControllerBase
{
    private static readonly Lazy<string> AssemblyVersion =
        new(() =>
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            return assemblyVersionAttribute?.InformationalVersion ??
                   assembly.GetName().Version?.ToString() ??
                   "N/A";
        });

    private static readonly string AssemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? "N/A";

    [AllowAnonymous]
    [HttpGet("", Name = nameof(GetAppInfo))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ApiInfo> GetAppInfo()
    {
        var baseUrl = configuration.GetValue<string>("BaseUrl") ?? "";
        var docs = $"{baseUrl.Trim('/')}/docs";

        var apiInfo = new ApiInfo(AssemblyName,
            AssemblyVersion.Value,
            hostEnvironment.EnvironmentName,
            docs);

        return Ok(apiInfo);
    }
}