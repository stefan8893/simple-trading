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
    public required DateTime OpenedAt { get; set; }
    public DateTime? FinishedAt { get; private set; }
    public Outcome? Outcome { get; private set; }
    public required Guid CurrencyId { get; init; }
    public required Currency Currency { get; init; }
    public required PositionPrices PositionPrices { get; init; }
    public double? RiskRewardRatio => PositionPrices.RiskRewardRatio;
    public ICollection<Reference> References { get; set; } = [];
    public string? Notes { get; set; }
    public bool IsFinished => Outcome is not null && FinishedAt.HasValue;
    public required DateTime CreatedAt { get; init; }

    internal OneOf<Completed, BusinessError> Finish(FinishTradeDto dto)
    {
        var finishedAtUpperBound = dto.UtcNow().AddDays(1);

        if (dto.FinishedAt < OpenedAt)
            return new BusinessError(Id, SimpleTradingStrings.FinishedBeforeOpenedError);

        if (dto.FinishedAt > finishedAtUpperBound)
            return new BusinessError(Id, SimpleTradingStrings.FinishedAtTooFarInTheFuture);

        switch (dto.Balance)
        {
            case < 0m when dto.Result != Result.Loss:
                return new BusinessError(Id, SimpleTradingStrings.LossIfBalanceIsLessThanZero);
            case 0m when dto.Result != Result.BreakEven:
                return new BusinessError(Id, SimpleTradingStrings.BreakEvenIfBalanceIsZero);
            case > 0m when dto.Result is Result.Loss or Result.BreakEven:
                return new BusinessError(Id, SimpleTradingStrings.MediocreOrWinIfBalanceAboveZero);
        }

        if (IsFinished)
            return CreateTradeAlreadyFinishedError(dto.TimeZone);

        var outcome = new Outcome
        {
            Balance = dto.Balance,
            Result = dto.Result
        };

        Outcome = outcome;
        FinishedAt = dto.FinishedAt.ToUtcKind();

        return new Completed();

        BusinessError CreateTradeAlreadyFinishedError(string timeZone)
        {
            var finishedAtLocal = FinishedAt!.Value.ToLocal(timeZone);
            var reason = string.Format(SimpleTradingStrings.TradeAlreadyFinished, finishedAtLocal);

            return new BusinessError(Id, reason);
        }
    }

    internal record FinishTradeDto(Result Result, decimal Balance, DateTime FinishedAt, UtcNow UtcNow, string TimeZone);
}