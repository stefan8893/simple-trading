using System.Net.Mime;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerUiControllerOrder(0)]
public class HomeController(IHostEnvironment hostEnvironment) : ControllerBase
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
    [HttpGet("info", Name = "GetAppInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ApiInfo> Info()
    {
        var apiInfo = new ApiInfo(AssemblyName,
            AssemblyVersion.Value,
            hostEnvironment.EnvironmentName);

        return Ok(apiInfo);
    }
}