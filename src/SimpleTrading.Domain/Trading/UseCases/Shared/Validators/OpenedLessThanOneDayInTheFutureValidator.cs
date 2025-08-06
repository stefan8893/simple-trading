using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Shared.Validators;

public record OpenedDateTime(DateTimeOffset? Opened);

[UsedImplicitly]
public class OpenedLessThanOneDayInTheFutureValidator : AbstractValidator<OpenedDateTime>
{
    public OpenedLessThanOneDayInTheFutureValidator(UtcNow utcNow, IUserSettingsRepository userSettingsRepository)
    {
        RuleFor(x => x)
            .CustomAsync(async (openedDateTime, ctx, cancellationToken) =>
            {
                var userSettings = await userSettingsRepository.GetUserSettings();
                var upperBound = utcNow().AddDays(Constants.OpenedDateMaxDaysInTheFutureBoundary);
                var upperBoundLocal = upperBound.ToLocal(userSettings.TimeZone).DateTime;

                if (openedDateTime?.Opened?.UtcDateTime > upperBound)
                    ctx.AddFailure(nameof(Trade.Opened), string.Format(SimpleTradingStrings.LessThanOrEqualValidatorMessage,
                        SimpleTradingStrings.Opened, upperBoundLocal.ToString("g")));
            })
            ;
    }
}