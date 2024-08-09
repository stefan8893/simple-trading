﻿using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class TradesController : ControllerBase
{
    [HttpGet("{tradeId:Guid}", Name = nameof(GetTrade))]
    [ProducesResponseType<TradeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetTrade(
        [FromServices] IGetTrade getTrade,
        Guid tradeId)
    {
        var result = await getTrade.Execute(tradeId);

        return result.Match(
            tradeModel => Ok(TradeDto.From(tradeModel)),
            notFound => notFound.ToActionResult()
        );
    }

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
            Opened = dto.Opened!.Value,
            Closed = dto.Closed,
            Size = dto.Size!.Value,
            Result = dto.Result.HasValue ? MapToDomainResult(dto.Result) : null,
            Balance = dto.Balance,
            CurrencyId = dto.CurrencyId!.Value,
            EntryPrice = dto.EntryPrice!.Value,
            StopLoss = dto.StopLoss,
            TakeProfit = dto.TakeProfit,
            ExitPrice = dto.ExitPrice,
            Notes = dto.Notes,
            References = dto.References?
                .Select(x =>
                    new AddTradeRequestModel.ReferenceModel(MapToDomainReferenceType(x.Type!.Value), x.Link!, x.Notes))
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

    [HttpPost("{tradeId:Guid}/close", Name = nameof(CloseTrade))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> CloseTrade(
        [FromServices] ICloseTrade closeTrade,
        [FromServices] IValidator<CloseTradeDto> validator,
        Guid tradeId,
        [FromBody] CloseTradeDto dto)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return validationResult.ToActionResult();

        var tradeResult = MapToDomainResult(dto.Result);

        var closeTradeRequestModel = new CloseTradeRequestModel(tradeId,
            tradeResult,
            dto.Balance!.Value,
            dto.ExitPrice!.Value,
            (DateTime) dto.Closed!);
        var result = await closeTrade.Execute(closeTradeRequestModel);

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
            _ => throw new ArgumentOutOfRangeException(nameof(resultDto), resultDto, null)
        };
        return tradeResult;
    }

    private static ReferenceType MapToDomainReferenceType(ReferenceTypeDto? typeDto)
    {
        var tradeResult = typeDto switch
        {
            ReferenceTypeDto.Other => ReferenceType.Other,
            ReferenceTypeDto.TradingView => ReferenceType.TradingView,
            _ => throw new ArgumentOutOfRangeException(nameof(typeDto), typeDto, null)
        };

        return tradeResult;
    }
}