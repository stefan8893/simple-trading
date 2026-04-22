using System.Collections.Immutable;
using JetBrains.Annotations;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.TradeResultAnalyzer;
using SimpleTrading.Domain.Trading.TradeResultAnalyzer.Decorators;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading;

[UsedImplicitly]
public class Trade : IEntity
{
    public required Guid AssetId { get; set; }
    public virtual required Asset Asset { get; set; }
    public required Guid ProfileId { get; set; }
    public virtual required Profile Profile { get; set; }
    public required DateTime Opened { get; set; }
    public required decimal Size { get; set; }
    public DateTime? Finished { get; private set; }
    public decimal? ProfitLoss { get; private set; }
    public Result? Result { get; private set; }
    public required Guid CurrencyId { get; set; }
    public virtual required Currency Currency { get; set; }
    public required PositionPrices PositionPrices { get; set; }
    public double? RiskRewardRatio => PositionPrices.RiskRewardRatio;
    public virtual ICollection<Reference> References { get; [UsedImplicitly] set; } = [];
    public string? Notes { get; set; }
    public bool IsFinished => Finished.HasValue && ProfitLoss.HasValue;
    public required Guid Id { get; init; }
    public required DateTime Created { get; init; }

    internal IImmutableList<string> GetWarnings()
    {
        if (!IsFinished)
            return ImmutableList<string>.Empty;

        var results = CalculateResults(new None());
        var calculatedResult =
            PickAppropriateResult(results.CalculatedByProfitLoss, results.CalculatedByPositionPrices);

        return AnalyzeResults(results, calculatedResult)
            .ToImmutableList();
    }

    internal OneOf<Completed<FinishTradeResult>, Conflict> RestoreCalculatedResult(UtcNow utcNow)
    {
        if (!IsFinished)
            return new Conflict(Id, SimpleTradingStrings.ResultOfAnOpenedTradeCannotBeReset);

        Result = null;
        return Finish(new FinishTradeConfiguration(Finished!.Value, ProfitLoss!.Value, utcNow));
    }

    internal OneOf<Completed<FinishTradeResult>, Conflict> Finish(FinishTradeConfiguration configuration)
    {
        if (configuration.Finished < Opened)
            return new Conflict(Id, SimpleTradingStrings.FinishedBeforeOpened);

        var utcNow = configuration.UtcNow();
        var finishedDateUpperBound =
            (Opened > utcNow ? Opened : utcNow).AddDays(Constants.OpenedDateMaxDaysInTheFutureBoundary);

        if (configuration.Finished > finishedDateUpperBound)
            return new Conflict(Id, SimpleTradingStrings.FinishedTooFarInTheFuture);

        return new Completed<FinishTradeResult>(FinishTrade(configuration));
    }

    private FinishTradeResult FinishTrade(FinishTradeConfiguration configuration)
    {
        Finished = configuration.Finished.ToUtcKind();
        ProfitLoss = configuration.ProfitLoss;

        if (configuration.ExitPrice.HasValue)
            PositionPrices.Exit = configuration.ExitPrice;

        var thereIsANewManuallyEnteredResult = configuration.ManuallyEnteredResult.IsT0;
        var currentResultWasManuallyEntered = Result?.Source == ResultSource.ManuallyEntered;

        var doNotOverrideResultThatWasPreviouslyManuallyEnteredWithANewCalculatedOne =
            IsFinished
            && currentResultWasManuallyEntered
            && !thereIsANewManuallyEnteredResult;

        var (result, warnings) = CalculateResult(configuration);

        if (doNotOverrideResultThatWasPreviouslyManuallyEnteredWithANewCalculatedOne)
            return new FinishTradeResult(Id, Result, warnings);

        Result = result;

        return new FinishTradeResult(Id, Result, warnings);
    }

    private (Result? result, IReadOnlyList<string> warnings) CalculateResult(FinishTradeConfiguration configuration)
    {
        var results = CalculateResults(configuration.ManuallyEnteredResult);
        var calculatedResult =
            PickAppropriateResult(results.CalculatedByProfitLoss, results.CalculatedByPositionPrices);
        var result = results.ManuallyEntered.Match(r => r, _ => calculatedResult);

        return (result, AnalyzeResults(results, calculatedResult));
    }

    private TradingResultsDto CalculateResults(OneOf<ResultModel?, None> manuallyEntered)
    {
        var manuallyEnteredResult = manuallyEntered
            .Match<OneOf<Result?, None>>(x => CreateManuallyEnteredResult(x), _ =>
                Result?.Source == ResultSource.ManuallyEntered
                    ? Result
                    : new None());

        var calculatedByProfitLoss = CalculateResultByProfitLoss(ProfitLoss!.Value);
        var calculatedByPositionPrices = PositionPrices.CalculateResult();

        return new TradingResultsDto(manuallyEnteredResult, calculatedByProfitLoss, calculatedByPositionPrices);
    }

    private Result? PickAppropriateResult(Result? profitLossResult,
        Result? positionPricesResult)
    {
        var hasProfitLossResult = profitLossResult is not null;
        var hasPositionPricesResult = positionPricesResult is not null;
        var isProfit = ProfitLoss!.Value > 0m;

        var positionPricesResultIsLossOrBreakEven =
            hasPositionPricesResult &&
            positionPricesResult?.Name is nameof(Result.Loss) or nameof(Result.BreakEven);

        if (isProfit && positionPricesResultIsLossOrBreakEven)
            return null;

        if (!hasProfitLossResult)
            return positionPricesResult;

        if (!hasPositionPricesResult)
            return profitLossResult;

        return positionPricesResult!.Name == profitLossResult!.Name
            // pick the result from position prices if both are equal
            // it contains more information (performance indicator)
            ? positionPricesResult
            // otherwise, pick the result by profit/loss, because it is more important than the result by position prices
            // at the end of the day the profit/loss counts and not position prices
            : profitLossResult;
    }

    private List<string> AnalyzeResults(TradingResultsDto results,
        Result? calculatedResult)
    {
        var enteredResultDiffersFromCalculatedResultAnalysis =
            new EnteredResultDiffersFromCalculatedAnalyzer();
        var longPositionResultAnalysisDecorator =
            new LongPositionAnalyzerDecorator(enteredResultDiffersFromCalculatedResultAnalysis);
        var shortPositionResultAnalysisDecorator =
            new ShortPositionTradeResultAnalyzerDecorator(longPositionResultAnalysisDecorator);
        var profitLossDiffersFromPositionPricesAnalysisDecorator =
            new ProfitLossDiffersFromPositionPricesAnalyzerDecorator(shortPositionResultAnalysisDecorator);

        var analyzeResultsConfiguration = new TradeResultAnalyzerConfiguration
        {
            ManuallyEntered = results.ManuallyEntered.Match(x => x, _ => null),
            CalculatedByProfitLoss = results.CalculatedByProfitLoss,
            CalculatedByPositionPrices = results.CalculatedByPositionPrices,
            CalculatedResult = calculatedResult
        };

        var analysisReport = profitLossDiffersFromPositionPricesAnalysisDecorator
            .AnalyzeResults(this, analyzeResultsConfiguration)
            .ToList();

        return analysisReport;
    }

    private static Result? CreateManuallyEnteredResult(ResultModel? resultModel)
    {
        return resultModel switch
        {
            ResultModel.Loss => new Result(Result.Loss, ResultSource.ManuallyEntered),
            ResultModel.BreakEven => new Result(Result.BreakEven, ResultSource.ManuallyEntered),
            ResultModel.Mediocre => new Result(Result.Mediocre, ResultSource.ManuallyEntered),
            ResultModel.Win => new Result(Result.Win, ResultSource.ManuallyEntered),
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(resultModel), resultModel, null)
        };
    }

    private static Result? CalculateResultByProfitLoss(decimal profitLoss)
    {
        return profitLoss switch
        {
            0m => new Result(Result.BreakEven, ResultSource.CalculatedByProfitLoss, 0),
            < 0m => new Result(Result.Loss, ResultSource.CalculatedByProfitLoss),
            _ => null
        };
    }

    private record TradingResultsDto(
        OneOf<Result?, None> ManuallyEntered,
        Result? CalculatedByProfitLoss,
        Result? CalculatedByPositionPrices);
}

internal record FinishTradeConfiguration(DateTime Finished, decimal ProfitLoss, UtcNow UtcNow)
{
    public decimal? ExitPrice { get; init; }
    public OneOf<ResultModel?, None> ManuallyEnteredResult { get; init; } = new None();
}

internal record FinishTradeResult(Guid TradeId, Result? Result, IEnumerable<string> Warnings);