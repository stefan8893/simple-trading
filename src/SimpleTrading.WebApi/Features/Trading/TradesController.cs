using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.Domain.Trading.UseCases.DeleteTrade;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.Trading.UseCases.UpdateTrade;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerOrder(1)]
public class TradesController : ControllerBase
{
    [HttpGet("{tradeId:guid}", Name = nameof(GetTrade))]
    [ProducesResponseType<TradeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetTrade([FromServices] IGetTrade getTrade, [FromRoute] Guid tradeId)
    {
        var result = await getTrade.Execute(new GetTradeRequestModel(tradeId));

        return result.Match(
            tradeModel => Ok(TradeDto.From(tradeModel)),
            notFound => notFound.ToActionResult()
        );
    }

    [HttpPost(Name = nameof(AddTrade))]
    [ProducesResponseType<SuccessResponse<Guid>>(StatusCodes.Status200OK)]
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
            completed => Ok(SuccessResponse<Guid>.From(completed.Data, completed.Warnings)),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );
    }

    [HttpPatch("{tradeId:guid}", Name = nameof(UpdateTrade))]
    [ProducesResponseType<SuccessResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> UpdateTrade(
        [FromServices] IUpdateTrade updateTrade,
        [FromRoute] Guid tradeId,
        [FromBody] UpdateTradeDto dto)
    {
        var updateTradeRequestModel = MapToRequestModel(tradeId, dto);
        var result = await updateTrade.Execute(updateTradeRequestModel);

        return result
            .Match(
                completed => Ok(SuccessResponse.From(completed.Warnings)),
                badInput => badInput.ToActionResult(),
                notFound => notFound.ToActionResult(),
                businessError => businessError.ToActionResult());
    }

    [HttpPut("{tradeId:guid}/close", Name = nameof(CloseTrade))]
    [ProducesResponseType<SuccessResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> CloseTrade(
        [FromServices] ICloseTrade closeTrade,
        [FromServices] IValidator<CloseTradeDto> validator,
        [FromRoute] Guid tradeId,
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
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );

        SuccessResponse<TradeResultDto> MapToSuccessResponse(Completed<CloseTradeResponseModel> completed)
        {
            return SuccessResponse<TradeResultDto>.From(TradeResultDto.From(completed.Data),
                completed.Warnings);
        }
    }

    [HttpDelete("{tradeId:guid}", Name = nameof(DeleteTrade))]
    [ProducesResponseType<SuccessResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<SuccessResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteTrade(
        [FromServices] IDeleteTrade deleteTrade,
        [FromRoute] Guid tradeId)
    {
        var result = await deleteTrade.Execute(new DeleteTradeRequestModel(tradeId));

        return result.Match(
            completed => Ok(SuccessResponse.From(completed.Warnings)),
            notFound => Ok(SuccessResponse.Empty)
        );
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
                    new ReferenceRequestModel(x.Type.ToDomainReferenceType(), x.Link!, x.Notes))
                .ToList() ?? []
        };

        return addTradeRequestModel;
    }

    private static UpdateTradeRequestModel MapToRequestModel(Guid tradeId, UpdateTradeDto dto)
    {
        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = tradeId,
            AssetId = dto.AssetId,
            ProfileId = dto.ProfileId,
            Opened = dto.Opened,
            Closed = dto.Closed,
            Size = dto.Size,
            Result = dto.Result is null ? new None() : MapToResultModel(dto.Result.Value),
            Balance = dto.Balance,
            CurrencyId = dto.CurrencyId,
            EntryPrice = dto.EntryPrice,
            StopLoss = dto.StopLoss is null ? new None() : dto.StopLoss.Value,
            TakeProfit = dto.TakeProfit is null ? new None() : dto.TakeProfit.Value,
            ExitPrice = dto.ExitPrice is null ? new None() : dto.ExitPrice.Value,
            Notes = dto.Notes is null ? new None() : dto.Notes.Value
        };
        return updateTradeRequestModel;
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
}