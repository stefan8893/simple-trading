using OneOf;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading;

public class Trade
{
    public required Guid Id { get; init; }
    public required Guid AssetId { get; set; }
    public required Asset Asset { get; set; }
    public required Guid ProfileId { get; set; }
    public required Profile Profile { get; set; }
    public required decimal Size { get; set; }
    public decimal? Balance { get; private set; }
    public Result? Result { get; private set; }
    public required DateTime Opened { get; set; }
    public DateTime? Closed { get; private set; }
    public required Guid CurrencyId { get; init; }
    public required Currency Currency { get; init; }
    public required PositionPrices PositionPrices { get; init; }
    public double? RiskRewardRatio => PositionPrices.RiskRewardRatio;
    public ICollection<Reference> References { get; set; } = [];
    public string? Notes { get; set; }
    public bool IsClosed => Closed.HasValue && Balance.HasValue;
    public required DateTime CreatedAt { get; init; }

    internal OneOf<Completed, BusinessError> Close(CloseTradeDto dto)
    {
        var closedUpperBound = dto.UtcNow().AddDays(1);

        if (dto.Closed < Opened)
            return new BusinessError(Id, SimpleTradingStrings.ClosedBeforeOpenedError);

        if (dto.Closed > closedUpperBound)
            return new BusinessError(Id, SimpleTradingStrings.ClosedTooFarInTheFuture);

        switch (dto.Balance)
        {
            case < 0m when dto.Result != Trading.Result.Loss:
                return new BusinessError(Id, SimpleTradingStrings.LossIfBalanceIsLessThanZero);
            case 0m when dto.Result != Trading.Result.BreakEven:
                return new BusinessError(Id, SimpleTradingStrings.BreakEvenIfBalanceIsZero);
            case > 0m when dto.Result is Trading.Result.Loss or Trading.Result.BreakEven:
                return new BusinessError(Id, SimpleTradingStrings.MediocreOrWinIfBalanceAboveZero);
        }

        if (IsClosed)
            return CreateTradeAlreadyClosedError(dto.TimeZone);

        Closed = dto.Closed.ToUtcKind();
        Balance = dto.Balance;
        Result = dto.Result;

        PositionPrices.ExitPrice = dto.ExitPrice;

        return new Completed();

        BusinessError CreateTradeAlreadyClosedError(string timeZone)
        {
            var closedLocal = Closed!.Value.ToLocal(timeZone);
            var reason = string.Format(SimpleTradingStrings.TradeAlreadyClosed, closedLocal);

            return new BusinessError(Id, reason);
        }
    }

    internal record CloseTradeDto(Result Result, decimal Balance, decimal ExitPrice, DateTime Closed, UtcNow UtcNow, string TimeZone);
}