using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
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
    [HttpPost(Name = nameof(AddTrade))]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> AddTrade(
        [FromServices] IAddTrade addTrade,
        [FromServices] IValidator<AddTradeDto> validator,
        [FromBody] AddTradeDto dto)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return validationResult.ToActionResult();

        var addTradeRequestModel = new AddTradeRequestModel
        {
            AssetId = dto.AssetId!.Value,
            ProfileId = dto.ProfileId!.Value,
            OpenedAt = dto.OpenedAt!.Value,
            FinishedAt = dto.FinishedAt,
            Size = dto.Size!.Value,
            Result = dto.Result.HasValue ? MapToDomainResult(dto.Result) : null,
            Balance = dto.Balance,
            CurrencyId = dto.CurrencyId!.Value,
            EntryPrice = dto.EntryPrice!.Value,
            StopLoss = dto.StopLoss,
            TakeProfit = dto.TakeProfit,
            Notes = dto.Notes,
            References = dto.References?
                .Select(x => new ReferenceModel(MapToDomainReferenceType(x.Type!.Value), x.Link!, x.Notes))
                .ToList() ?? []
        };

        var result = await addTrade.Execute(addTradeRequestModel);

        return result.Match(
            completed => Ok(completed.Data),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );
    }

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

        var tradeResult = MapToDomainResult(dto.Result);

        var finishTradeRequestModel = new FinishTradeRequestModel(tradeId, tradeResult, (decimal) dto.Balance!,
            (DateTime) dto.FinishedAt!);
        var result = await finishTrade.Execute(finishTradeRequestModel);

        return result.Match(
            completed => NoContent(),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );
    }

    private static Result MapToDomainResult(ResultDto? resultDto)
    {
        var tradeResult = resultDto switch
        {
            ResultDto.Win => Result.Win,
            ResultDto.Mediocre => Result.Mediocre,
            ResultDto.BreakEven => Result.BreakEven,
            ResultDto.Loss => Result.Loss,
            _ => throw new ArgumentOutOfRangeException(nameof(ResultDto))
        };
        return tradeResult;
    }

    private static ReferenceType MapToDomainReferenceType(ReferenceTypeDto? typeDto)
    {
        var tradeResult = typeDto switch
        {
            ReferenceTypeDto.Other => ReferenceType.Other,
            ReferenceTypeDto.TradingView => ReferenceType.TradingView,
            _ => throw new ArgumentOutOfRangeException(nameof(ResultDto))
        };

        return tradeResult;
    }
}