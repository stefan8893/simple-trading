using System.Globalization;
using FluentValidation;
using JetBrains.Annotations;
using NodaTime;
using OneOf;
using OneOf.Types;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.User.UseCases.UpdateUserLanguage;

public record UpdateUserSettingsRequestModel(string? Culture, OneOf<string?, None> IsoLanguageCode, string? Timezone);

[UsedImplicitly]
public class UpdateUserLanguageRequestModelValidator : AbstractValidator<UpdateUserSettingsRequestModel>
{
    public UpdateUserLanguageRequestModelValidator()
    {
        RuleFor(x => x.Culture)
            .Must(x => Constants.SupportedCultures.Contains(new CultureInfo(x!)))
            .WithMessage(x =>
                string.Format(SimpleTradingStrings.CultureNotSupported,
                    x.Culture,
                    string.Join(", ", Constants.SupportedCultures.Select(c => c.Name).Order(StringComparer.InvariantCultureIgnoreCase))))
            .When(x => x.Culture is not null);

        RuleFor(x => x.IsoLanguageCode.AsT0)
            .Must(x => Constants.SupportedLanguages.Contains(x!))
            .WithMessage(x =>
                string.Format(SimpleTradingStrings.LanguageNotSupported,
                    x.IsoLanguageCode.AsT0,
                    string.Join(", ", Constants.SupportedLanguages.Order(StringComparer.InvariantCultureIgnoreCase))))
            .OverridePropertyName(x => x.IsoLanguageCode)
            .When(x => x.IsoLanguageCode is {IsT0: true, AsT0: not null});

        RuleFor(x => x.Timezone)
            .Must(x => DateTimeZoneProviders.Tzdb.GetZoneOrNull(x!) is not null)
            .WithMessage(x =>
                string.Format(SimpleTradingStrings.TimezoneIsInvalid, x.Timezone))
            .When(x => x.Timezone is not null);
    }
}