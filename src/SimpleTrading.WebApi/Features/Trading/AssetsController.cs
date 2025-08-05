using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading.UseCases.Assets.GetAssets;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class AssetsController : SimpleControllerBase
{
    [HttpGet(Name = nameof(GetAssets))]
    [ProducesResponseType<IEnumerable<AssetDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> GetAssets(
        [FromServices] IGetAssets getAssets,
        [FromQuery] string? searchTerm)
    {
        var result = await getAssets
            .Execute(new GetAssetsRequestModel(searchTerm));

        return result.Match(
            assets => Ok(assets.Select(AssetDto.From)),
            UnprocessableEntityResult
        );
    }
}