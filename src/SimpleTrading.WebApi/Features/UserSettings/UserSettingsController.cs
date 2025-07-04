using System.Globalization;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.TimeZones;
using OneOf.Types;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.User.UseCases.GetUserSettings;
using SimpleTrading.Domain.User.UseCases.UpdateUserSettings;
using SimpleTrading.WebApi.Extensions;
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

    [HttpGet("available-timezones", Name = nameof(GetAvailableTimezones))]
    [ProducesResponseType<IEnumerable<TimeZoneOption>>(StatusCodes.Status200OK)]
    public ActionResult GetAvailableTimezones()
    {
        var timezoneOptions = TzdbDateTimeZoneSource
            .Default
            .WindowsToTzdbIds
            .Select(x =>
            {
                var now = SystemClock.Instance.GetCurrentInstant();
                var offset = DateTimeZoneProviders.Tzdb[x.Value].GetUtcOffset(now);
                var offsetFormatted = offset.ToString("m", CultureInfo.InvariantCulture);

                return new TimeZoneOption(x.Key, x.Value, offsetFormatted);
            });

        return Ok(timezoneOptions);
    }

    [HttpPatch(Name = nameof(UpdateUserSettings))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateUserSettings(
        [FromServices] IUpdateUserSettings updateUserSettings,
        [FromBody] UpdateUserSettingsDto updateUserSettingsDto)
    {
        var requestModel = new UpdateUserSettingsRequestModel(updateUserSettingsDto.Culture,
            updateUserSettingsDto.IsoLanguageCode is not null
                ? updateUserSettingsDto.IsoLanguageCode.Value
                : new None(),
            updateUserSettingsDto.TimeZone);
        var result = await updateUserSettings.Execute(requestModel);

        return result.Match(
            completed => NoContent(),
            badInput => badInput.ToActionResult());
    }
}