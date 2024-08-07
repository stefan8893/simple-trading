﻿using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.WebApi.Features.Trading.DTOs;

public record AddTradeDto
{
    public Guid? AssetId { get; set; }
    public Guid? ProfileId { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public decimal? Size { get; set; }
    public ResultDto? Result { get; set; }
    public decimal? Balance { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal? EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<ReferenceDto>? References { get; set; }
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

        RuleFor(x => x.OpenedAt)
            .NotNull()
            .WithName(SimpleTradingStrings.OpenedAt);

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