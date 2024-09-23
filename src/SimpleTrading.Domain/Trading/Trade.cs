using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.TradeResultAnalyzer;
using SimpleTrading.Domain.Trading.TradeResultAnalyzer.Decorators;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading;

public class Trade : IEntity
{
    public required Guid AssetId { get; set; }
    public virtual required Asset Asset { get; set; }
    public required Guid ProfileId { get; set; }
    public virtual required Profile Profile { get; set; }
    public required DateTime Opened { get; set; }
    public required decimal Size { get; set; }
    public DateTime? Closed { get; private set; }
    public decimal? Balance { get; private set; }
    public Result? Result { get; private set; }
    public required Guid CurrencyId { get; set; }
    public virtual required Currency Currency { get; set; }
    public required PositionPrices PositionPrices { get; set; }
    public double? RiskRewardRatio => PositionPrices.RiskRewardRatio;
    public virtual ICollection<Reference> References { get; set; } = [];
    public string? Notes { get; set; }
    public bool IsClosed => Closed.HasValue && Balance.HasValue;
    public required Guid Id { get; init; }
    public required DateTime Created { get; init; }

    internal OneOf<Completed, BusinessError> RestoreCalculatedResult(UtcNow utcNow)
    {
        if (!IsClosed)
            return new BusinessError(Id, SimpleTradingStrings.ResultOfAnOpenedTradeCannotBeReset);

        Result = null;
        return Close(new CloseTradeConfiguration(Closed!.Value, Balance!.Value, utcNow));
    }

    internal OneOf<Completed, BusinessError> Close(CloseTradeConfiguration configuration)
    {
        if (configuration.Closed < Opened)
            return new BusinessError(Id, SimpleTradingStrings.ClosedBeforeOpened);

        var utcNow = configuration.UtcNow();
        var closedDateUpperBound =
            (Opened > utcNow ? Opened : utcNow).AddDays(Constants.OpenedDateMaxDaysInTheFutureLimit);

        if (configuration.Closed > closedDateUpperBound)
            return new BusinessError(Id, SimpleTradingStrings.ClosedTooFarInTheFuture);

        return CloseTrade(configuration);
    }

    private Completed CloseTrade(CloseTradeConfiguration configuration)
    {
        Closed = configuration.Closed.ToUtcKind();
        Balance = configuration.Balance;

        if (configuration.ExitPrice.HasValue)
            PositionPrices.Exit = configuration.ExitPrice;
        
        var thereIsANewManuallyEnteredResult = configuration.ManuallyEnteredResult.IsT0;
        var currentResultWasManuallyEntered = Result?.Source == ResultSource.ManuallyEntered;

        var doNotOverrideResultThatWasPreviouslyManuallyEnteredWithANewCalculatedOne =
            IsClosed
            && currentResultWasManuallyEntered
            && !thereIsANewManuallyEnteredResult;

        var (result, warnings) = CalculateResult(configuration);

        if (doNotOverrideResultThatWasPreviouslyManuallyEnteredWithANewCalculatedOne)
            return new Completed(warnings);

        Result = result;

        return new Completed(warnings);
    }

    private (Result? result, IReadOnlyList<Warning> warnings) CalculateResult(CloseTradeConfiguration configuration)
    {
        var results = CalculateResults(configuration);
        var calculatedResult = PickAppropriateResult(results.CalculatedByBalance, results.CalculatedByPositionPrices);
        var result = results.ManuallyEntered.Match(r => r, _ => calculatedResult);

        return (result, AnalyzeResults(results, calculatedResult));
    }

    private TradingResultsDto CalculateResults(CloseTradeConfiguration configuration)
    {
        var manuallyEnteredResult = configuration.ManuallyEnteredResult
            .Match<OneOf<Result?, None>>(x => CreateManuallyEnteredResult(x), _ =>
                Result?.Source == ResultSource.ManuallyEntered
                    ? Result
                    : new None());

        var calculatedByBalance = CalculateResultByBalance(Balance!.Value);
        var calculatedByPositionPrices = PositionPrices.CalculateResult();

        return new TradingResultsDto(manuallyEnteredResult, calculatedByBalance, calculatedByPositionPrices);
    }

    private Result? PickAppropriateResult(Result? balanceResult,
        Result? positionPricesResult)
    {
        var hasBalanceResult = balanceResult is not null;
        var hasPositionPricesResult = positionPricesResult is not null;
        var isPositiveBalance = Balance!.Value > 0m;

        var positionPricesResultIsLossOrBreakEven =
            hasPositionPricesResult &&
            positionPricesResult?.Name is nameof(Result.Loss) or nameof(Result.BreakEven);

        if (isPositiveBalance && positionPricesResultIsLossOrBreakEven)
            return null;

        if (!hasBalanceResult)
            return positionPricesResult;

        if (!hasPositionPricesResult)
            return balanceResult;

        return positionPricesResult!.Name == balanceResult!.Name
            // pick the result from position prices if both are equal
            // it contains more information (performance indicator)
            ? positionPricesResult
            // otherwise pick the result by balance, because it is more important than the result by position prices
            // at the end of the day the balance counts and not position prices
            : balanceResult;
    }

    private List<Warning> AnalyzeResults(TradingResultsDto results,
        Result? calculatedResult)
    {
        var enteredResultDiffersFromCalculatedResultAnalysis =
            new EnteredResultDiffersFromCalculatedAnalyzer();
        var longPositionResultAnalysisDecorator =
            new LongPositionAnalyzerDecorator(enteredResultDiffersFromCalculatedResultAnalysis);
        var shortPositionResultAnalysisDecorator =
            new ShortPositionTradeResultAnalyzerDecorator(longPositionResultAnalysisDecorator);
        var balanceDiffersFromPositionPricesAnalysisDecorator =
            new BalanceDiffersFromPositionPricesAnalyzerDecorator(shortPositionResultAnalysisDecorator);

        var analyzeResultsConfiguration = new TradeResultAnalyzerConfiguration
        {
            ManuallyEntered = results.ManuallyEntered.Match(x => x, _ => null),
            CalculatedByBalance = results.CalculatedByBalance,
            CalculatedByPositionPrices = results.CalculatedByPositionPrices,
            CalculatedResult = calculatedResult
        };

        var analysisReport = balanceDiffersFromPositionPricesAnalysisDecorator
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

    private static Result? CalculateResultByBalance(decimal balance)
    {
        return balance switch
        {
            0m => new Result(Result.BreakEven, ResultSource.CalculatedByBalance, 0),
            < 0m => new Result(Result.Loss, ResultSource.CalculatedByBalance),
            _ => null
        };
    }

    private record TradingResultsDto(
        OneOf<Result?, None> ManuallyEntered,
        Result? CalculatedByBalance,
        Result? CalculatedByPositionPrices);
}

internal record CloseTradeConfiguration(DateTime Closed, decimal Balance, UtcNow UtcNow)
{
    public decimal? ExitPrice { get; init; }
    public OneOf<ResultModel?, None> ManuallyEnteredResult { get; init; } = new None();
}