using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases;
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
    [ProducesResponseType<SuccessResponse<TradeAddedDto>>(StatusCodes.Status200OK)]
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

        var addTradeRequestModel = MapToRequestModel(dto);
        var result = await addTrade.Execute(addTradeRequestModel);

        return result.Match(
            completed => Ok(MapToSuccessResponse(completed)),
            completedWithWarnings => Ok(MapToSuccessResponseWithWarnings(completedWithWarnings)),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );

        SuccessResponse<TradeAddedDto> MapToSuccessResponse(Completed<AddTradeResponseModel> completed)
        {
            return SuccessResponse<TradeAddedDto>.From(TradeAddedDto.From(completed.Data));
        }

        SuccessResponse<TradeAddedDto> MapToSuccessResponseWithWarnings(
            CompletedWithWarnings<AddTradeResponseModel> completedWithWarnings)
        {
            return SuccessResponse<TradeAddedDto>.From(TradeAddedDto.From(completedWithWarnings.Data),
                completedWithWarnings.Warnings);
        }
    }

    [HttpPost("{tradeId:Guid}/close", Name = nameof(CloseTrade))]
    [ProducesResponseType<SuccessResponse>(StatusCodes.Status200OK)]
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

        var tradeResult = MapToResultModel(dto.Result);

        var closeTradeRequestModel = new CloseTradeRequestModel(tradeId,
            dto.Closed!.Value,
            dto.Balance!.Value,
            tradeResult,
            dto.ExitPrice);
        var result = await closeTrade.Execute(closeTradeRequestModel);

        return result.Match(
            completed => Ok(MapToSuccessResponse(completed)),
            completedWithWarnings => Ok(MapToSuccessResponseWithWarnings(completedWithWarnings)),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );

        SuccessResponse<TradeResultDto> MapToSuccessResponse(Completed<CloseTradeResponseModel> completed)
        {
            return SuccessResponse<TradeResultDto>.From(TradeResultDto.From(completed.Data));
        }

        SuccessResponse<TradeResultDto> MapToSuccessResponseWithWarnings(
            CompletedWithWarnings<CloseTradeResponseModel> completedWithWarnings)
        {
            return SuccessResponse<TradeResultDto>.From(TradeResultDto.From(completedWithWarnings.Data),
                completedWithWarnings.Warnings);
        }
    }

    private static AddTradeRequestModel MapToRequestModel(AddTradeDto dto)
    {
        var addTradeRequestModel = new AddTradeRequestModel
        {
            AssetId = dto.AssetId!.Value,
            ProfileId = dto.ProfileId!.Value,
            Opened = dto.Opened!.Value,
            Closed = dto.Closed,
            Size = dto.Size!.Value,
            Result = dto.Result.HasValue ? MapToResultModel(dto.Result) : null,
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
        return addTradeRequestModel;
    }

    private static ResultModel? MapToResultModel(ResultDto? resultDto)
    {
        var tradeResult = resultDto switch
        {
            ResultDto.Win => ResultModel.Win,
            ResultDto.Mediocre => ResultModel.Mediocre,
            ResultDto.BreakEven => ResultModel.BreakEven,
            ResultDto.Loss => ResultModel.Loss,
            _ => (ResultModel?) null
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