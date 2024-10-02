using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.Shared.Validators;

[UsedImplicitly]
public class OpenedLessThanOneDayInTheFutureValidator : AbstractValidator<DateTimeOffset?>
{
    public OpenedLessThanOneDayInTheFutureValidator(UtcNow utcNow, IUserSettingsRepository userSettingsRepository)
    {
        RuleFor(x => x)
            .CustomAsync(async (opened, ctx, cancellationToken) =>
            {
                var userSettings = await userSettingsRepository.GetUserSettings();
                var upperBound = utcNow().AddDays(Constants.OpenedDateMaxDaysInTheFutureBoundary);
                var upperBoundLocal = upperBound.ToLocal(userSettings.TimeZone).DateTime;

                if (opened?.UtcDateTime > upperBound)
                    ctx.AddFailure(string.Format(SimpleTradingStrings.LessThanOrEqualValidatorMessage,
                        SimpleTradingStrings.Opened, upperBoundLocal.ToString("g")));
            });
    }
}