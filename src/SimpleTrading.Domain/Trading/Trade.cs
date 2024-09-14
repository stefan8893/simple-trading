using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.TradeResultAnalyser;
using SimpleTrading.Domain.Trading.TradeResultAnalyser.Decorators;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading;

public class Trade : IEntity
{
    public required Guid Id { get; init; }
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
    public required DateTime Created { get; init; }

    internal OneOf<Completed, BusinessError> ResetManuallyEnteredResult(UtcNow utcNow)
    {
        if (!IsClosed)
            return new BusinessError(Id, SimpleTradingStrings.ResultOfAnOpenedTradeCannotBeReset);

        Result = null;
        return Close(new CloseTradeDto(Closed!.Value, Balance!.Value, utcNow));
    }

    internal OneOf<Completed, BusinessError> Close(CloseTradeDto dto)
    {
        if (dto.Closed < Opened)
            return new BusinessError(Id, SimpleTradingStrings.ClosedBeforeOpened);

        var closedDateUpperBound = dto.UtcNow().AddDays(1);
        if (dto.Closed > closedDateUpperBound)
            return new BusinessError(Id, SimpleTradingStrings.ClosedTooFarInTheFuture);

        if (IsClosed && Result?.Source == ResultSource.ManuallyEntered)
            return new Completed();

        Closed = dto.Closed.ToUtcKind();
        Balance = dto.Balance;

        if (dto.ExitPrice.HasValue)
            PositionPrices.Exit = dto.ExitPrice;

        var results = CalculateResults(dto);
        var calculatedResult = PickAppropriateResult(results.CalculatedByBalance, results.CalculatedByPositionPrices);
        Result = results.ManuallyEntered ?? calculatedResult;

        return AnalyseResults(results, calculatedResult);
    }

    private TradingResultsDto CalculateResults(CloseTradeDto dto)
    {
        var manuallyEnteredResult = dto.Result.HasValue
            ? CreateManuallyEnteredResult(dto.Result.Value)
            // do not lose a manual result that was previously entered
            : Result?.Source == ResultSource.ManuallyEntered
                ? Result
                : null;

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
            // pick the result from the position prices if both are equal
            // it contains more information (performance indicator)
            ? positionPricesResult
            // otherwise pick the result by balance, because it is more important than the result by position prices
            // at the end of the day counts the balance and not position prices
            : balanceResult;
    }

    private Completed AnalyseResults(TradingResultsDto results,
        Result? calculatedResult)
    {
        var enteredResultDiffersFromCalculatedResultAnalysis =
            new EnteredResultDiffersFromCalculatedAnalyser();
        var longPositionResultAnalysisDecorator =
            new LongPositionAnalyserDecorator(enteredResultDiffersFromCalculatedResultAnalysis);
        var shortPositionResultAnalysisDecorator =
            new ShortPositionTradeResultAnalyserDecorator(longPositionResultAnalysisDecorator);
        var balanceDiffersFromPositionPricesAnalysisDecorator =
            new BalanceDiffersFromPositionPricesAnalyserDecorator(shortPositionResultAnalysisDecorator);

        var analyseResultsConfiguration = new TradeResultAnalyserConfiguration
        {
            ManuallyEntered = results.ManuallyEntered,
            CalculatedByBalance = results.CalculatedByBalance,
            CalculatedByPositionPrices = results.CalculatedByPositionPrices,
            CalculatedResult = calculatedResult
        };

        var analysisResult = balanceDiffersFromPositionPricesAnalysisDecorator
            .AnalyseResults(this, analyseResultsConfiguration)
            .ToList();

        return new Completed(analysisResult);
    }

    private static Result CreateManuallyEnteredResult(ResultModel resultModel)
    {
        return resultModel switch
        {
            ResultModel.Loss => new Result(Result.Loss, ResultSource.ManuallyEntered),
            ResultModel.BreakEven => new Result(Result.BreakEven, ResultSource.ManuallyEntered, 0),
            ResultModel.Mediocre => new Result(Result.Mediocre, ResultSource.ManuallyEntered),
            ResultModel.Win => new Result(Result.Win, ResultSource.ManuallyEntered),
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

    internal record CloseTradeDto(DateTime Closed, decimal Balance, UtcNow UtcNow)
    {
        public decimal? ExitPrice { get; init; }
        public ResultModel? Result { get; init; }
    }

    private record TradingResultsDto(
        Result? ManuallyEntered,
        Result? CalculatedByBalance,
        Result? CalculatedByPositionPrices);
}