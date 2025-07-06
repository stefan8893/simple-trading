using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.Domain.Trading.UseCases.DeleteTrade;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.Trading.UseCases.UpdateTrade;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerPosition(1)]
public partial class TradesController : ControllerBase
{
    [HttpGet(Name = nameof(SearchTrades))]
    [ProducesResponseType<PageDto<TradeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
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
            badInput => badInput.ToActionResult()
        );
    }

    [HttpGet("{tradeId:guid}", Name = nameof(GetTrade))]
    [ProducesResponseType<TradeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetTrade([FromServices] IGetTrade getTrade, [FromRoute] Guid tradeId)
    {
        var result = await getTrade.Execute(new GetTradeRequestModel(tradeId));

        return result.Match(
            tradeModel => Ok(TradeDto.From(tradeModel)),
            notFound => notFound.ToActionResult()
        );
    }

    [HttpPost(Name = nameof(AddTrade))]
    [ProducesResponseType<AddTradeResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> AddTrade(
        [FromServices] IAddTrade addTrade,
        [FromBody] AddTradeDto addTradeDto)
    {
        var addTradeRequestModel = MapToRequestModel(addTradeDto);
        var result = await addTrade.Execute(addTradeRequestModel);

        return result.Match(
            completed => Ok(AddTradeResultDto.From(completed.Data)),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );
    }

    [HttpPatch("{tradeId:guid}", Name = nameof(UpdateTrade))]
    [ProducesResponseType<WarningsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
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
                completed => Ok(new WarningsDto(completed.Data.Warnings)),
                badInput => badInput.ToActionResult(),
                notFound => notFound.ToActionResult(),
                businessError => businessError.ToActionResult());
    }

    [HttpPut("{tradeId:guid}/close", Name = nameof(CloseTrade))]
    [ProducesResponseType<TradeResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> CloseTrade(
        [FromServices] ICloseTrade closeTrade,
        [FromRoute] Guid tradeId,
        [FromBody] CloseTradeDto closeTradeDto)
    {
        OneOf<ResultModel?, None> tradeResult = closeTradeDto.ManuallyEnteredResult is null
            ? new None()
            : MapToResultModel(closeTradeDto.ManuallyEnteredResult.Value);

        var closeTradeRequestModel = new CloseTradeRequestModel(tradeId,
            closeTradeDto.Closed!.Value,
            closeTradeDto.Balance!.Value)
        {
            ManuallyEnteredResult = tradeResult,
            ExitPrice = closeTradeDto.ExitPrice
        };
        var result = await closeTrade.Execute(closeTradeRequestModel);

        return result.Match(
            completed => Ok(TradeResultDto.From(completed.Data)),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );
    }

    [HttpPut("{tradeId:guid}/restore-calculated-result", Name = nameof(RestoreCalculatedResult))]
    [ProducesResponseType<TradeResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> RestoreCalculatedResult(
        [FromServices] IRestoreCalculatedResult restoreCalculatedResult,
        [FromRoute] Guid tradeId)
    {
        var result = await restoreCalculatedResult.Execute(new RestoreCalculatedResultRequestModel(tradeId));

        return result.Match(
            completed => Ok(TradeResultDto.From(completed.Data)),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult());
    }

    [HttpDelete("{tradeId:guid}", Name = nameof(DeleteTrade))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteTrade(
        [FromServices] IDeleteTrade deleteTrade,
        [FromRoute] Guid tradeId)
    {
        _ = await deleteTrade.Execute(new DeleteTradeRequestModel(tradeId));

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
            : literal.Value.IsNullLiteral()
                ? literal.Value
                : throw new Exception($"Invalid literal '{literal.Value}'.");
    }

    private static AddTradeRequestModel MapToRequestModel(AddTradeDto dto)
    {
        return new AddTradeRequestModel
        {
            AssetId = dto.AssetId!.Value,
            ProfileId = dto.ProfileId!.Value,
            Opened = dto.Opened!.Value,
            Closed = dto.Closed,
            Size = dto.Size!.Value,
            ManuallyEnteredResult = dto.ManuallyEnteredResult is null
                ? new None()
                : MapToResultModel(dto.ManuallyEnteredResult.Value),
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
    }

    private static UpdateTradeRequestModel MapToRequestModel(Guid tradeId, UpdateTradeDto dto)
    {
        return new UpdateTradeRequestModel
        {
            TradeId = tradeId,
            AssetId = dto.AssetId,
            ProfileId = dto.ProfileId,
            Opened = dto.Opened,
            Closed = dto.Closed,
            Size = dto.Size,
            ManuallyEnteredResult = dto.ManuallyEnteredResult is null
                ? new None()
                : MapToResultModel(dto.ManuallyEnteredResult.Value),
            Balance = dto.Balance,
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
        @"\s*(?<property>.*?)\s+\-(?<operator>.*?)\s+(?<literal>(?i)null(?-i)|\[(?<comparisonValue>.*?)\])\s*$")]
    public static partial Regex PropertyFilterSyntaxRegex();
}