using FluentValidation;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public enum ResultDto
{
    Win,
    Mediocre,
    BreakEven,
    Loss
}

public class CloseTradeDto
{
    public decimal? Balance { get; set; }
    public decimal? ExitPrice { get; set; }
    public DateTimeOffset? Closed { get; set; }
    public UpdateValue<ResultDto?>? ManuallyEnteredResult { get; set; }
}

public record TradeResultDto(Guid TradeId, ResultDto? Result, short? Performance)
{
    public static TradeResultDto From(CloseTradeResponseModel model)
    {
        return new TradeResultDto(model.TradeId, model.Result.ToResultDto(), model.Performance);
    }
    
    public static TradeResultDto From(RestoreCalculatedResultResponseModel model)
    {
        return new TradeResultDto(model.TradeId, model.Result.ToResultDto(), model.Performance);
    }
}

public class CloseTradeDtoValidator : AbstractValidator<CloseTradeDto>
{
    public CloseTradeDtoValidator()
    {
        RuleFor(x => x.Balance)
            .NotNull()
            .WithName(SimpleTradingStrings.Balance);

        RuleFor(x => x.Closed)
            .NotNull()
            .WithName(SimpleTradingStrings.Closed);
    }
}