using FluentValidation;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeReferenceRequestModel(Guid Id, ReferenceType Type, string Link, string? Notes = null)
    : ReferenceModel(Id, Type, Link, Notes);

public record AddTradeRequestModel
{
    public required Guid AssetId { get; init; }
    public required Guid ProfileId { get; init; }
    public required DateTimeOffset Opened { get; init; }
    public DateTimeOffset? Closed { get; set; }
    public required decimal Size { get; init; }
    public ResultModel? Result { get; set; }
    public decimal? Balance { get; set; }
    public required Guid CurrencyId { get; init; }
    public required decimal EntryPrice { get; init; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public decimal? ExitPrice { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<ReferenceRequestModel> References { get; set; } = [];
}

public class AddTradeRequestModelValidator : AbstractValidator<AddTradeRequestModel>
{
    public AddTradeRequestModelValidator(UtcNow utcNow, ReferenceRequestModelValidator referenceRequestModelValidator,
        IUserSettingsRepository userSettingsRepository)
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Asset);

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Profile);

        RuleFor(x => x.Opened.DateTime)
            .GreaterThanOrEqualTo(Constants.MinDate)
            .OverridePropertyName(x => x.Opened)
            .WithName(SimpleTradingStrings.Opened);

        RuleFor(x => x.Opened)
            .CustomAsync(async (opened, ctx, cancellationToken) =>
            {
                var userSettings = await userSettingsRepository.Get();
                var upperBound = utcNow().AddDays(Constants.OpenedDateMaxDaysInTheFutureLimit);
                var upperBoundLocal = upperBound.ToLocal(userSettings.TimeZone).DateTime;

                if (opened.UtcDateTime > upperBound)
                    ctx.AddFailure(string.Format(SimpleTradingStrings.LessThanOrEqualValidatorMessage,
                        SimpleTradingStrings.Opened, upperBoundLocal.ToString("g")));
            });

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TradeSize);

        RuleFor(x => x.Result)
            .IsInEnum()
            .WithName(SimpleTradingStrings.Result)
            .When(x => x.Result.HasValue);

        RuleFor(x => x.Balance)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Balance)
            .WithMessage(string.Format(SimpleTradingStrings.XMustNotBeEmptyIfYIsThere, "{PropertyName}",
                SimpleTradingStrings.Closed))
            .When(x => x.Closed.HasValue);

        RuleFor(x => x.Closed)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Closed)
            .WithMessage(string.Format(SimpleTradingStrings.XMustNotBeEmptyIfYIsThere, "{PropertyName}",
                SimpleTradingStrings.Balance))
            .When(x => x.Balance.HasValue);

        RuleFor(x => x.EntryPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.EntryPrice);

        RuleFor(x => x.StopLoss)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.StopLoss)
            .When(x => x.StopLoss.HasValue);

        RuleFor(x => x.TakeProfit)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.TakeProfit)
            .When(x => x.TakeProfit.HasValue);

        RuleFor(x => x.ExitPrice)
            .GreaterThan(0)
            .WithName(SimpleTradingStrings.ExitPrice)
            .When(x => x.ExitPrice.HasValue);

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithName(SimpleTradingStrings.Currency);

        RuleFor(x => x.Notes)
            .MaximumLength(4000)
            .WithName(SimpleTradingStrings.Notes);

        RuleForEach(x => x.References)
            .SetValidator(referenceRequestModelValidator);
    }
}