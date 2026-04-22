using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
using SimpleTrading.Domain.Trading.UseCases.FinishTrade;
using SimpleTrading.Domain.Trading.UseCases.DeleteTrade;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.Trading.UseCases.UpdateTrade;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public partial class TradesController : SimpleControllerBase
{
    [HttpGet(Name = nameof(SearchTrades))]
    [ProducesResponseType<PageDto<TradeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SearchTrades(
        [FromServices] ISearchTrades searchTrades,
        [FromQuery] SearchQueryDto searchQueryDto)
    {
        var searchTradesRequestModel = MapToRequestModel(searchQueryDto);

        var result = await searchTrades.Execute(searchTradesRequestModel);

        return result.Match(
            page => Ok(new PageDto<TradeDto>(
                Enumerable.Select(page, TradeDto.From),
                page.Count,
                page.TotalCount,
                page.TotalPages,
                page.Page,
                page.PageSize)),
            UnprocessableEntityResult
        );
    }

    [HttpGet("{tradeId:guid}", Name = nameof(GetTrade))]
    [ProducesResponseType<TradeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetTrade([FromServices] IGetTrade getTrade, [FromRoute] Guid tradeId)
    {
        var result = await getTrade.Execute(tradeId);

        return result.Match(
            tradeModel => Ok(TradeDto.From(tradeModel)),
            NotFoundResult
        );
    }

    [HttpPost(Name = nameof(AddTrade))]
    [ProducesResponseType<AddTradeResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddTrade(
        [FromServices] IAddTrade addTrade,
        [FromBody] AddTradeDto addTradeDto,
        [FromQuery] bool? dryRun = false)
    {
        var addTradeRequestModel = MapToRequestModel(addTradeDto, dryRun);
        var result = await addTrade.Execute(addTradeRequestModel);

        return result.Match(
            completed => Ok(AddTradeResultDto.From(completed.Data)),
            NotFoundResult,
            ConflictResult,
            UnprocessableEntityResult
        );
    }

    [HttpPatch("{tradeId:guid}", Name = nameof(UpdateTrade))]
    [ProducesResponseType<WarningsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateTrade(
        [FromServices] IUpdateTrade updateTrade,
        [FromRoute] Guid tradeId,
        [FromBody] UpdateTradeDto dto)
    {
        var updateTradeRequestModel = MapToRequestModel(tradeId, dto);
        var result = await updateTrade.Execute(updateTradeRequestModel);

        return result
            .Match(
                completed => Ok(new WarningsDto(completed.Data.Warnings)),
                NotFoundResult,
                ConflictResult,
                UnprocessableEntityResult);
    }

    [HttpPut("{tradeId:guid}/finish", Name = nameof(FinishTrade))]
    [ProducesResponseType<TradeResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> FinishTrade(
        [FromServices] IFinishTrade finishTrade,
        [FromRoute] Guid tradeId,
        [FromBody] FinishTradeDto finishTradeDto)
    {
        OneOf<ResultModel?, None> tradeResult = finishTradeDto.ManuallyEnteredResult is null
            ? new None()
            : MapToResultModel(finishTradeDto.ManuallyEnteredResult.Value);

        var finishTradeRequestModel = new FinishTradeRequestModel(tradeId,
            finishTradeDto.Finished!.Value,
            finishTradeDto.ProfitLoss!.Value)
        {
            ManuallyEnteredResult = tradeResult,
            ExitPrice = finishTradeDto.ExitPrice
        };
        var result = await finishTrade.Execute(finishTradeRequestModel);

        return result.Match(
            completed => Ok(TradeResultDto.From(completed.Data)),
            NotFoundResult,
            ConflictResult,
            UnprocessableEntityResult
        );
    }

    [HttpPut("{tradeId:guid}/restore-calculated-result", Name = nameof(RestoreCalculatedResult))]
    [ProducesResponseType<TradeResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RestoreCalculatedResult(
        [FromServices] IRestoreCalculatedResult restoreCalculatedResult,
        [FromRoute] Guid tradeId)
    {
        var result = await restoreCalculatedResult.Execute(tradeId);

        return result.Match(
            completed => Ok(TradeResultDto.From(completed.Data)),
            NotFoundResult,
            ConflictResult);
    }

    [HttpDelete("{tradeId:guid}", Name = nameof(DeleteTrade))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteTrade(
        [FromServices] IDeleteTrade deleteTrade,
        [FromRoute] Guid tradeId)
    {
        await deleteTrade.Execute(tradeId);

        return NoContent();
    }

    private static SearchTradesRequestModel MapToRequestModel(SearchQueryDto queryDto)
    {
        var searchTradesRequestModel = new SearchTradesRequestModel
        {
            ProfileId = queryDto.ProfileId!.Value,
            Sort = queryDto.Sort?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(ParseSorting)
                .ToList() ?? [],
            Filter = queryDto.Filter?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                {
                    var match = PropertyFilterSyntaxRegex().Match(x);
                    return new FilterModel
                    {
                        PropertyName = match.Groups["property"].Value,
                        Operator = match.Groups["operator"].Value,
                        ComparisonValue = GetComparisonValue(match),
                        IsLiteral = IsLiteral(match)
                    };
                })
                .ToList() ?? []
        };

        if (queryDto.Page.HasValue)
            searchTradesRequestModel.Page = queryDto.Page.Value;

        if (queryDto.PageSize.HasValue)
            searchTradesRequestModel.PageSize = queryDto.PageSize.Value;

        return searchTradesRequestModel;

        SortModel ParseSorting(string sortBy)
        {
            var sortByTrimmed = sortBy.Trim();

            return sortByTrimmed.StartsWith('-')
                ? new SortModel(sortByTrimmed[1..], false)
                : new SortModel(sortByTrimmed);
        }
    }

    private static bool IsLiteral(Match match)
    {
        var comparisonValue = match.Groups["comparisonValue"];
        var literal = match.Groups["literal"];

        return !comparisonValue.Success && literal.Success;
    }

    private static string GetComparisonValue(Match match)
    {
        var comparisonValue = match.Groups["comparisonValue"];
        var literal = match.Groups["literal"];

        return comparisonValue.Success
            ? comparisonValue.Value
            : literal.Value.IsNullLiteral() || literal.Value.IsBoolLiteral()
                ? literal.Value
                : throw new Exception($"Invalid literal '{literal.Value}'.");
    }

    private static AddTradeRequestModel MapToRequestModel(AddTradeDto dto, bool? dryRun)
    {
        return new AddTradeRequestModel
        {
            DryRun = dryRun ?? false,
            AssetId = dto.AssetId!.Value,
            ProfileId = dto.ProfileId!.Value,
            Opened = dto.Opened!.Value,
            Finished = dto.Finished,
            Size = dto.Size!.Value,
            ManuallyEnteredResult = dto.ManuallyEnteredResult is null
                ? new None()
                : MapToResultModel(dto.ManuallyEnteredResult.Value),
            ProfitLoss = dto.ProfitLoss,
            CurrencyId = dto.CurrencyId!.Value,
            EntryPrice = dto.EntryPrice!.Value,
            StopLoss = dto.StopLoss,
            TakeProfit = dto.TakeProfit,
            ExitPrice = dto.ExitPrice,
            Notes = dto.Notes,
            References = dto.References?
                .Select(x =>
                    new ReferenceRequestModel(x.Link!, x.Notes))
                .ToList() ?? []
        };
    }

    private static UpdateTradeRequestModel MapToRequestModel(Guid tradeId, UpdateTradeDto dto)
    {
        return new UpdateTradeRequestModel
        {
            TradeId = tradeId,
            AssetId = dto.AssetId,
            ProfileId = dto.ProfileId,
            Opened = dto.Opened,
            Finished = dto.Finished,
            Size = dto.Size,
            ManuallyEnteredResult = dto.ManuallyEnteredResult is null
                ? new None()
                : MapToResultModel(dto.ManuallyEnteredResult.Value),
            ProfitLoss = dto.ProfitLoss,
            CurrencyId = dto.CurrencyId,
            EntryPrice = dto.EntryPrice,
            StopLoss = dto.StopLoss is null ? new None() : dto.StopLoss.Value,
            TakeProfit = dto.TakeProfit is null ? new None() : dto.TakeProfit.Value,
            ExitPrice = dto.ExitPrice is null ? new None() : dto.ExitPrice.Value,
            Notes = dto.Notes is null ? new None() : dto.Notes.Value
        };
    }

    private static ResultModel? MapToResultModel(ResultDto? resultDto)
    {
        return resultDto switch
        {
            ResultDto.Win => ResultModel.Win,
            ResultDto.Mediocre => ResultModel.Mediocre,
            ResultDto.BreakEven => ResultModel.BreakEven,
            ResultDto.Loss => ResultModel.Loss,
            _ => null
        };
    }

    [GeneratedRegex(
        @"\s*(?<property>.*?)\s+\-(?<operator>.*?)\s+(?<literal>(?i)null|true|false(?-i)|\[(?<comparisonValue>.*?)\])\s*$")]
    public static partial Regex PropertyFilterSyntaxRegex();
}