using FluentValidation;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.AddTrade;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;

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
    public DateTime? Closed { get; set; }
    public ResultDto? Result { get; set; }
}

public record TradeResultDto(ResultDto? Result, short? Performance)
{
    public static TradeResultDto From(CloseTradeResponseModel model)
    {
        return new TradeResultDto(model.Result.ToResultDto(), model.Performance);
    }

    public static TradeResultDto From(AddTradeResponseModel model)
    {
        return new TradeResultDto(model.Result.ToResultDto(), model.Performance);
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

        RuleFor(x => x.Result)
            .IsInEnum()
            .When(x => x.Result is not null);
    }
}
