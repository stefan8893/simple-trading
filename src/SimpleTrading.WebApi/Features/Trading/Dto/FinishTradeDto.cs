using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;
using SimpleTrading.WebApi.Features.Dto;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class FinishTradeDto
{
    public decimal? ProfitLoss { get; set; }
    public decimal? ExitPrice { get; set; }
    public DateTimeOffset? Finished { get; set; }
    public UpdateResultValue? ManuallyEnteredResult { get; set; }
}

[UsedImplicitly]
public class FinishTradeDtoValidator : AbstractValidator<FinishTradeDto>
{
    public FinishTradeDtoValidator()
    {
        RuleFor(x => x.ProfitLoss)
            .NotNull()
            .WithName(SimpleTradingStrings.ProfitLoss);

        RuleFor(x => x.Finished)
            .NotNull()
            .WithName(SimpleTradingStrings.Finished);
    }
}