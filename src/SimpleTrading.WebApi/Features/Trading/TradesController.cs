using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.FinishTrade;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.DTOs;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class TradesController : ControllerBase
{
    [HttpPost("{tradeId:Guid}/finish", Name = nameof(FinishTrade))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> FinishTrade(
        [FromServices] IFinishTrade finishTrade,
        [FromServices] IValidator<FinishTradeDto> validator,
        Guid tradeId,
        [FromBody] FinishTradeDto dto)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return validationResult.ToActionResult();

        var tradeResult = dto.Result switch
        {
            ResultDto.Win => Result.Win,
            ResultDto.Mediocre => Result.Mediocre,
            ResultDto.BreakEven => Result.BreakEven,
            ResultDto.Loss => Result.Loss,
            _ => throw new ArgumentOutOfRangeException(nameof(dto.Result))
        };

        var model = new FinishTradeRequestModel(tradeId, tradeResult, (decimal)dto.Balance!, (DateTime)dto.FinishedAt!);
        var result = await finishTrade.Execute(model);

        return result.Match(
            completed => completed.ToActionResult(),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );
    }
}