using FluentValidation;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeRequestModel(
    Guid AssetId,
    Guid ProfileId,
    DateTimeOffset OpenedAt,
    DateTimeOffset? FinishedAt,
    decimal Size,
    Result? Result,
    decimal Balance,
    Guid CurrencyId,
    decimal EntryPrice,
    decimal? StopLoss,
    decimal? TakeProfit,
    string? Notes,
    IReadOnlyList<ReferenceModel> References);
public record ReferenceModel(ReferenceType ReferenceType, string Link, string Description);

public class AddTradeRequestModelValidator : AbstractValidator<AddTradeRequestModel>
{
    public AddTradeRequestModelValidator()
    {
        RuleFor(x => x.OpenedAt)
            .GreaterThanOrEqualTo(Constants.MinDate);

        RuleFor(x => x.FinishedAt)
            .GreaterThanOrEqualTo(Constants.MinDate)
            .LessThanOrEqualTo(x => x.OpenedAt)
            .When(x => x is not null);

        RuleFor(x => x.Result).IsInEnum();
    }
}
