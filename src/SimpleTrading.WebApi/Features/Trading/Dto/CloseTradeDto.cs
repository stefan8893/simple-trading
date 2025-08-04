using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class CloseTradeDto
{
    public decimal? ProfitLoss { get; set; }
    public decimal? ExitPrice { get; set; }
    public DateTimeOffset? Closed { get; set; }
    public UpdateResultValue? ManuallyEnteredResult { get; set; }
}

[UsedImplicitly]
public class CloseTradeDtoValidator : AbstractValidator<CloseTradeDto>
{
    public CloseTradeDtoValidator()
    {
        RuleFor(x => x.ProfitLoss)
            .NotNull()
            .WithName(SimpleTradingStrings.ProfitLoss);

        RuleFor(x => x.Closed)
            .NotNull()
            .WithName(SimpleTradingStrings.Closed);
    }
}