using FluentValidation;
using SimpleTrading.Domain.Resources;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public record AddTradeDto
{
    /// <example>0c275c78-0508-4836-81d5-342e2445d60c</example>
    public Guid? AssetId { get; set; }

    /// <example>401c519b-956a-4a5f-bd84-77e716817771</example>
    public Guid? ProfileId { get; set; }

    public DateTimeOffset? Opened { get; set; }

    public DateTimeOffset? Closed { get; set; }

    /// <example>5000</example>
    public decimal? Size { get; set; }
    
    public ResultDto? Result { get; set; }

    /// <example>125</example>
    public decimal? Balance { get; set; }

    /// <example>dd1f1281-7ec9-450e-8dd8-da1f4eb78629</example>
    public Guid? CurrencyId { get; set; }

    /// <example>1.0</example>
    public decimal? EntryPrice { get; set; }

    /// <example>0.9</example>
    public decimal? StopLoss { get; set; }

    /// <example>1.3</example>
    public decimal? TakeProfit { get; set; }

    /// <example>1.25</example>
    public decimal? ExitPrice { get; set; }

    /// <example>null</example>
    public string? Notes { get; set; }

    /// <example>null</example>
    public IReadOnlyList<AddReferenceDto>? References { get; set; }
}

public class AddTradeDtoValidator : AbstractValidator<AddTradeDto>
{
    public AddTradeDtoValidator()
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
    }
}