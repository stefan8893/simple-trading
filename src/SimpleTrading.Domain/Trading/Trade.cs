using OneOf;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.TradeResultAnalyser;
using SimpleTrading.Domain.Trading.TradeResultAnalyser.Decorators;
using SimpleTrading.Domain.Trading.UseCases;

namespace SimpleTrading.Domain.Trading;

public class Trade
{
    public required Guid Id { get; init; }
    public required Guid AssetId { get; set; }
    public required Asset Asset { get; set; }
    public required Guid ProfileId { get; set; }
    public required Profile Profile { get; set; }
    public required DateTime Opened { get; set; }
    public required decimal Size { get; set; }
    public decimal? Balance { get; private set; }
    public ITradingResult? Result { get; private set; }
    public DateTime? Closed { get; private set; }
    public required Guid CurrencyId { get; init; }
    public required Currency Currency { get; init; }
    public required PositionPrices PositionPrices { get; init; }
    public double? RiskRewardRatio => PositionPrices.RiskRewardRatio;
    public ICollection<Reference> References { get; set; } = [];
    public string? Notes { get; set; }
    public bool IsClosed => Closed.HasValue && Balance.HasValue;
    public required DateTime Created { get; init; }

    internal OneOf<Completed, BusinessError> Close(CloseTradeDto dto)
    {
        if (dto.Closed < Opened)
            return new BusinessError(Id, SimpleTradingStrings.ClosedBeforeOpenedError);

        var closedUpperBound = dto.UtcNow().AddDays(1);
        if (dto.Closed > closedUpperBound)
            return new BusinessError(Id, SimpleTradingStrings.ClosedTooFarInTheFutureError);

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
            : null;

        var calculatedByBalance = CalculateResultByBalance(Balance!.Value);
        var calculatedByPositionPrices = PositionPrices.CalculateResult();

        return new TradingResultsDto(manuallyEnteredResult, calculatedByBalance, calculatedByPositionPrices);
    }

    private ITradingResult? PickAppropriateResult(ITradingResult? balanceResult,
        ITradingResult? positionPricesResult)
    {
        var hasBalanceResult = balanceResult is not null;
        var hasPositionPricesResult = positionPricesResult is not null;
        var isPositiveBalance = Balance!.Value > 0m;

        var positionPricesResultIsLossOrBreakEven =
            hasPositionPricesResult &&
            positionPricesResult?.Name is nameof(TradingResult.Loss) or nameof(TradingResult.BreakEven);

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
            // prefer the result by balance if there is a difference to the result by position prices
            : balanceResult;
    }

    private Completed AnalyseResults(TradingResultsDto results,
        ITradingResult? calculatedResult)
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

    private static ITradingResult CreateManuallyEnteredResult(ResultModel resultModel)
    {
        return resultModel switch
        {
            ResultModel.Loss => new TradingResult.Loss(TradingResultSource.ManuallyEntered),
            ResultModel.BreakEven => new TradingResult.BreakEven(TradingResultSource.ManuallyEntered),
            ResultModel.Mediocre => new TradingResult.Mediocre(TradingResultSource.ManuallyEntered),
            ResultModel.Win => new TradingResult.Win(TradingResultSource.ManuallyEntered),
            _ => throw new ArgumentOutOfRangeException(nameof(resultModel), resultModel, null)
        };
    }

    private static ITradingResult? CalculateResultByBalance(decimal balance)
    {
        return balance switch
        {
            0m => new TradingResult.BreakEven(TradingResultSource.CalculatedByBalance, 0),
            < 0m => new TradingResult.Loss(TradingResultSource.CalculatedByBalance),
            _ => null
        };
    }

    internal record CloseTradeDto(DateTime Closed, decimal Balance, UtcNow UtcNow)
    {
        public decimal? ExitPrice { get; init; }
        public ResultModel? Result { get; init; }
    }

    private record TradingResultsDto(
        ITradingResult? ManuallyEntered,
        ITradingResult? CalculatedByBalance,
        ITradingResult? CalculatedByPositionPrices);
}