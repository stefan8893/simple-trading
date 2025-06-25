using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.User.UseCases.UpdateUserLanguage;

public record UpdateUserLanguageRequestModel(string? IsoLanguageCode);

[UsedImplicitly]
public class UpdateUserLanguageRequestModelValidator : AbstractValidator<UpdateUserLanguageRequestModel>
{
    public UpdateUserLanguageRequestModelValidator()
    {
        RuleFor(x => x.IsoLanguageCode)
            .Must(x => Constants.SupportedLanguages.Contains(x!))
            .WithMessage(x =>
                string.Format(SimpleTradingStrings.LanguageNotSupported,
                    x.IsoLanguageCode,
                    string.Join(", ", Constants.SupportedLanguages.Order(StringComparer.InvariantCultureIgnoreCase))))
            .When(x => x.IsoLanguageCode is not null);
    }
}