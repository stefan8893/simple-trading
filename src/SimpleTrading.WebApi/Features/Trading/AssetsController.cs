using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerPosition(4)]
public class AssetsController : ControllerBase
{
    [HttpGet(Name = nameof(GetAssets))]
    [ProducesResponseType<IEnumerable<AssetDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetAssets(
        [FromServices] IGetAssets getAssets,
        [FromQuery] string? searchTerm)
    {
        var result = await getAssets
            .Execute(new GetAssetsRequestModel(searchTerm));

        return result.Match(
            assets => Ok(assets.Select(AssetDto.From)),
            badInput => badInput.ToActionResult()
        );
    }
}