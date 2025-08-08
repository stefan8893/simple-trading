using SimpleTrading.WebApi.Features.Dto;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class UpdateTradeDto
{
    public Guid? AssetId { get; set; }
    public Guid? ProfileId { get; set; }
    public DateTimeOffset? Opened { get; set; }
    public DateTimeOffset? Closed { get; set; }
    public decimal? Size { get; set; }
    public UpdateResultValue? ManuallyEnteredResult { get; set; }
    public decimal? ProfitLoss { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal? EntryPrice { get; set; }
    public UpdateValue<decimal?>? StopLoss { get; set; }
    public UpdateValue<decimal?>? TakeProfit { get; set; }
    public UpdateValue<decimal?>? ExitPrice { get; set; }
    public UpdateStringValue? Notes { get; set; }
}