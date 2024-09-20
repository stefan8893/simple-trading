using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.UseCases.GetUserSettings;
using SimpleTrading.WebApi.Features.UserSettings.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.UserSettings;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerPosition(6)]
public class UserSettingsController : ControllerBase
{
    [HttpGet(Name = nameof(GetUserSettings))]
    [ProducesResponseType<UserSettingsDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetUserSettings([FromServices] IGetUserSettings getUserSettings)
    {
        var userSettings = await getUserSettings.Execute();

        return Ok(UserSettingsDto.From(userSettings));
    }

    [HttpGet("local-now", Name = nameof(GetUserLocalNow))]
    [ProducesResponseType<DateTimeOffset>(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetUserLocalNow([FromServices] LocalNow localNow)
    {
        var nowInUserTimeZone = await localNow();

        return Ok(nowInUserTimeZone);
    }
}