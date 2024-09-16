using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.UserSettings;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerPosition(6)]
public class UserSettingsController : ControllerBase
{
    [HttpGet("local-now", Name = "GetUserLocalNow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetLocalNow([FromServices] LocalNow localNow)
    {
        var nowInUserTimeZone = await localNow();
        return Ok(nowInUserTimeZone);
    }
}