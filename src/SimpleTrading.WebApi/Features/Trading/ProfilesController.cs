using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading.UseCases.Profiles.GetActiveProfile;
using SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProfilesController : SimpleControllerBase
{
    [HttpGet(Name = nameof(GetProfiles))]
    [ProducesResponseType<IEnumerable<ProfileDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> GetProfiles(
        [FromServices] IGetProfiles getProfiles,
        [FromQuery] string? searchTerm)
    {
        var result = await getProfiles
            .Execute(new GetProfilesRequestModel(searchTerm));

        return result.Match(
            profiles => Ok(profiles.Select(ProfileDto.From)),
            UnprocessableEntityResult
        );
    }

    [HttpGet("active", Name = nameof(GetActiveProfile))]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetActiveProfile([FromServices] IGetActiveProfile getActiveProfile)
    {
        var activeProfile = await getActiveProfile.Execute();

        return Ok(ProfileDto.From(activeProfile));
    }
}