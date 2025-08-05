using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public record AddTradeDto
{
    public bool? DryRun { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ProfileId { get; set; }
    public DateTimeOffset? Opened { get; set; }
    public DateTimeOffset? Closed { get; set; }
    public decimal? Size { get; set; }
    public UpdateResultValue? ManuallyEnteredResult { get; set; }
    public decimal? ProfitLoss { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal? EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public decimal? ExitPrice { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<AddReferenceDto>? References { get; set; }
}

[UsedImplicitly]
public class AddTradeDtoValidator : AbstractValidator<AddTradeDto>
{
    public AddTradeDtoValidator(AddReferenceDtoValidator addReferenceDtoValidator)
    {
        RuleFor(x => x.AssetId)
            .NotNull()
            .WithName(SimpleTradingStrings.Asset);

        RuleFor(x => x.ProfileId)
            .NotNull()
            .WithName(SimpleTradingStrings.Profile);

        RuleFor(x => x.Opened)
            .NotNull()
            .WithName(SimpleTradingStrings.Opened);

        RuleFor(x => x.Size)
            .NotNull()
            .WithName(SimpleTradingStrings.TradeSize);

        RuleFor(x => x.CurrencyId)
            .NotNull()
            .WithName(SimpleTradingStrings.Currency);

        RuleFor(x => x.EntryPrice)
            .NotNull()
            .WithName(SimpleTradingStrings.EntryPrice);
        
        RuleForEach(x => x.References)
            .SetValidator(addReferenceDtoValidator);
    }
}