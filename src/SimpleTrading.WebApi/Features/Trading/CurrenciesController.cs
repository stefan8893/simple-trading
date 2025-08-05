using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CurrenciesController : SimpleControllerBase
{
    [HttpGet(Name = nameof(GetCurrencies))]
    [ProducesResponseType<IEnumerable<CurrencyDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> GetCurrencies(
        [FromServices] IGetCurrencies getCurrencies,
        [FromQuery] string? searchTerm)
    {
        var result = await getCurrencies
            .Execute(new GetCurrenciesRequestModel(searchTerm));

        return result.Match(
            currencies => Ok(currencies.Select(CurrencyDto.From)),
            UnprocessableEntityResult
        );
    }
}