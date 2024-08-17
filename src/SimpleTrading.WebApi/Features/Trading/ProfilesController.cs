using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerPosition(3)]
public class ProfilesController : ControllerBase
{
    [HttpGet(Name = nameof(GetProfiles))]
    [ProducesResponseType<IEnumerable<ProfileDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetProfiles(
        [FromServices] IGetProfiles getProfiles,
        [FromQuery] string? searchTerm)
    {
        var result = await getProfiles
            .Execute(new GetProfilesRequestModel(searchTerm));

        return result.Match(
            profiles => Ok(profiles.Select(ProfileDto.From)),
            badInput => badInput.ToActionResult()
        );
    }
}