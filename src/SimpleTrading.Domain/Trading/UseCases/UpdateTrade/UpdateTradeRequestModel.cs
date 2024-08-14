using FluentValidation;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.UseCases.UpdateTrade;

public record UpdateTradeRequestModel
{
    public required Guid TradeId { get; init; }
    public Guid? AssetId { get; init; }
    public Guid? ProfileId { get; init; }
    public DateTimeOffset? Opened { get; init; }
    public DateTimeOffset? Closed { get; set; }
    public decimal? Size { get; init; }
    public OneOf<ResultModel?, None> Result { get; set; }
    public decimal? Balance { get; set; }
    public Guid? CurrencyId { get; init; }
    public decimal? EntryPrice { get; init; }
    public OneOf<decimal?, None> StopLoss { get; set; }
    public OneOf<decimal?, None> TakeProfit { get; set; }
    public OneOf<decimal?, None> ExitPrice { get; set; }
    public OneOf<string?, None> Notes { get; set; }
}

public class UpdateTradeRequestModelValidator : AbstractValidator<UpdateTradeRequestModel>
{
    public UpdateTradeRequestModelValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Asset)
            .When(x => x.AssetId.HasValue);

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Profile)
            .When(x => x.ProfileId.HasValue);

        RuleFor(x => x.Opened)
            .GreaterThanOrEqualTo(Constants.MinDate)
            .WithName(SimpleTradingStrings.Opened)
            .When(x => x.Opened.HasValue);

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TradeSize)
            .When(x => x.Size.HasValue);

        RuleFor(x => x.Result.AsT0)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .WithState(x => new CustomPropertyName(nameof(x.Result)))
            .When(x => x.Result is {IsT0: true, AsT0: not null});

        RuleFor(x => x.EntryPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.EntryPrice)
            .When(x => x.EntryPrice.HasValue);

        RuleFor(x => x.StopLoss.AsT0)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.StopLoss)
            .WithState(x => new CustomPropertyName(nameof(x.StopLoss)))
            .When(x => x.StopLoss is {IsT0: true, AsT0: not null});

        RuleFor(x => x.TakeProfit.AsT0)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TakeProfit)
            .WithState(x => new CustomPropertyName(nameof(x.TakeProfit)))
            .When(x => x.TakeProfit is {IsT0: true, AsT0: not null});

        RuleFor(x => x.ExitPrice.AsT0)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.ExitPrice)
            .WithState(x => new CustomPropertyName(nameof(x.ExitPrice)))
            .When(x => x.ExitPrice is {IsT0: true, AsT0: not null});

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Currency)
            .When(x => x.CurrencyId.HasValue);
        
    }
}
