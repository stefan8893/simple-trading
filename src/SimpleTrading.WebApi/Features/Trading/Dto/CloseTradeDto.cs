using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;
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

[UsedImplicitly]
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