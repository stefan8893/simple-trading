using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerOrder(5)]
public class CurrenciesController : ControllerBase
{
    [HttpGet(Name = nameof(GetCurrencies))]
    [ProducesResponseType<IEnumerable<CurrencyDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetCurrencies(
        [FromServices] IGetCurrencies getCurrencies,
        [FromQuery] string? searchTerm)
    {
        var result = await getCurrencies
            .Execute(new GetCurrenciesRequestModel(searchTerm));

        return result.Match(
            currencies => Ok(currencies.Select(CurrencyDto.From)),
            badInput => badInput.ToActionResult()
        );
    }
}